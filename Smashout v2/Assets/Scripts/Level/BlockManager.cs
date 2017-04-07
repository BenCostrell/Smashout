using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {
    public List<GameObject> blockTypes;
    public List<GameObject> blockPatterns;
    public List<GameObject> blockStatics;
    public float blockDeathTime;
    public float blockAppearanceTime;
    public float minAcceptableDistance;
    public int maxNumTries;
    public int blockCountLow;
    public int blockCountHigh;
    public int patternCountLow;
    public int patternCountHigh;
    public float playerSpawnPlatformOffset;
    public bool genPlayerSpawnPlatforms;
    public bool pause;

    public Action init
    {
        get { return _init; }
        set { _init = (value == null ? DefaultInit : value); }
    }
    public Action behaviour
    {
        get { return _behaviour; }
        set { _behaviour = (value == null ? DefaultBehaviour : value); }
    }

    private Action _init;
    private Action _behaviour;
    private List<Block> blocks;
    private UnityEngine.Random.State preInitRNG;
    private int blockCount;
    private int patternCount;

    void Start()
    {
        //Defaults _init and _behaviour if they have not already been set
        init = _init;
        behaviour = _behaviour;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        if (!pause) _behaviour();
    }

    public void GenerateLevel()
    {
        init = _init;
        _init();
    }

    public void Restart()
    {
        DestroyAllBlocks(false);
        UnityEngine.Random.state = preInitRNG;
        GenerateLevel();
    }

    public void Restart(int seed)
    {
        UnityEngine.Random.InitState(seed);
        preInitRNG = UnityEngine.Random.state;
        Restart();
    }

    private void DefaultInit()
    {
        GenerateInitialBlockSetup();
        StartAppearanceOfAllBlocks();
    }

    private void DefaultBehaviour()
    {

    }

    Vector3 GenerateLocation()
    {
        float x = UnityEngine.Random.Range(transform.position.x - transform.localScale.x / 2.0f, transform.position.x + transform.localScale.x / 2.0f);
        float y = UnityEngine.Random.Range(transform.position.y - transform.localScale.y / 2.0f, transform.position.y + transform.localScale.y / 2.0f);
        return new Vector3(x, y, 0);
    }

    Block Create(Vector3 location, GameObject blockType)
    {
        GameObject obj = Instantiate(blockType, location, Quaternion.identity) as GameObject;
        Block block = obj.GetComponent<Block>();
        block.GetComponent<SpriteRenderer>().enabled = false;
        return block;
    }

    Block Create(Vector3 location, int blockType = -1)
    {
        if(blockType == -1)
        {
            UnityEngine.Random.State oldState = UnityEngine.Random.state;
            blockType = UnityEngine.Random.Range(0, Math.Max(0, blockTypes.Count-1));
            UnityEngine.Random.state = oldState;
        }
        return Create(location, blockTypes[blockType]);
    }

    bool ValidateLocation(Vector3 location)
    {
        bool valid = true;
        foreach (Block block in blocks)
        {
            if (Vector3.Distance(block.transform.position, location) < minAcceptableDistance)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }

    bool ValidateLocation(Bounds bounds)
    {
        foreach (Block block in blocks)
        {
            Vector3 refPoint = Vector3.zero;
            if (block.transform.position.x > bounds.max.x) refPoint += new Vector3(bounds.max.x, 0);
            else if (block.transform.position.x < bounds.min.x) refPoint += new Vector3(bounds.min.x, 0);
            else return false;

            if (block.transform.position.y > bounds.max.y) refPoint += new Vector3(0, bounds.max.y);
            else if (block.transform.position.y < bounds.min.y) refPoint += new Vector3(0, bounds.min.y);
            else return false;

            if (Vector3.Distance(block.transform.position, refPoint) < minAcceptableDistance) return false;
        }
        return true;
    }

    Vector3 GenerateValidLocation(GameObject pattern)
    {
        GameObject obj = Instantiate(pattern, Vector3.zero, Quaternion.identity) as GameObject;
        Bounds bounds = new TotalBounds(obj);
        bounds.center = GenerateLocation();
        bool valid = ValidateLocation(bounds);

        for (int i = 0; i < maxNumTries; i++)
        {
            if (!valid)
            {
                bounds.center = GenerateLocation();
                valid = ValidateLocation(bounds);
            }
            else
            {
                break;
            }
        }
        if (!valid)
        {
            bounds.center = Vector3.forward;
        }

        Destroy(obj);

        return bounds.center;
    }

    Vector3 GenerateValidLocation()
    {
        Vector3 location = GenerateLocation();
        bool valid = ValidateLocation(location);
        for (int i = 0; i < maxNumTries; i++)
        {
            if (!valid)
            {
                location = GenerateLocation();
                valid = ValidateLocation(location);
            }
            else
            {
                break;
            }
        }
        if (!valid)
        {
            location = Vector3.forward;
        }
        return location;
    }

    Block GenerateValidBlock(int type = -1)
    {
        Vector3 location = GenerateValidLocation();
        Block block;
        block = Create(location, type);
        return block;
    }

    public void GenerateInitialBlockSetup()
    {
        blockCount = UnityEngine.Random.Range(blockCountLow, blockCountHigh);
        patternCount = UnityEngine.Random.Range(patternCountLow, patternCountHigh);

        preInitRNG = UnityEngine.Random.state;

        int blockGenCount = 0;
        int patternGenCount = 0;
        int staticsGenCount = 0;
        int spawnPlatformsGenCount = 0;

        blocks = new List<Block>();
        Block block;

        if(genPlayerSpawnPlatforms)
        {
            foreach (Vector3 loc in Services.GameManager.spawnpoints)
            {
                blocks.Add(Create(loc - Vector3.up * playerSpawnPlatformOffset, 0));
                ++spawnPlatformsGenCount;
            }
        }

        foreach (GameObject o in blockStatics)
        {
            GameObject prefab = Instantiate(o) as GameObject;
            foreach (Transform b in prefab.GetComponentsInChildren<Transform>())
            {
                if (b.gameObject == prefab && b.gameObject.GetComponent<Block>() == null) continue;

                b.parent = null;
                blocks.Add(b.GetComponent<Block>());
            }
            ++staticsGenCount;
        }

        if (blockPatterns.Count != 0)
        {
            for (int i = 0; i < patternCount; i++)
            {
                GameObject patternType = blockPatterns[UnityEngine.Random.Range(0, Math.Max(0, blockPatterns.Count - 1))];
                Vector3 location = GenerateValidLocation(patternType);
                if (location == Vector3.forward)
                {
                    Debug.Log("After " + maxNumTries + " tries, failed to place pattern \"" + patternType.name + "\". Decreasing pattern count and trying again with a new random pattern.");
                    continue;
                }

                GameObject pattern = Instantiate(patternType, location, Quaternion.identity) as GameObject;
                foreach (Transform b in pattern.GetComponentsInChildren<Transform>())
                {
                    if (b.gameObject == pattern) continue;

                    b.parent = null;
                    blocks.Add(b.GetComponent<Block>());
                }
                Destroy(pattern);
                ++patternGenCount;
            }
        }
        else Debug.Log("Attempted to make " + patternCount + " patterns, but no patterns were available to select.");

        if (blockTypes.Count != 0)
        {
            for (int i = 0; i < blockCount; i++)
            {
                block = GenerateValidBlock();
                if (block.transform.position == Vector3.forward)
                {
                    Destroy(block.gameObject);
                    Debug.Log("After " + maxNumTries + " tries, only made " + i + "blocks.");
                    break;
                }
                else
                {
                    blocks.Add(block);
                }
                ++blockGenCount;
            }
        }
        else Debug.Log("Attempted to make " + blockCount + " blocks, but no block types were available to select.");

        Debug.Log("Level Generation Info" + "\n" +
        "---------------------" + "\n" +
        "Generation Area: (" + (transform.position.x - transform.localScale.x / 2.0f) + ", " +
        (transform.position.y - transform.localScale.y / 2.0f) + ") to (" +
        (transform.position.x + transform.localScale.x / 2.0f) + ", " +
        (transform.position.y + transform.localScale.y / 2.0f) + ")" + "\n" +
        "Max Attempts: " + maxNumTries + "\n" +
        "Random Block Gen Target: " + blockCount + "\n" +
        "Pattern Gen Target: " + patternCount + "\n" +
        "Blocks Statistics:" + "\n" +
        "  Random Blocks Generated:\t" + blockGenCount + "\n" +
        "  Patterns Generated:\t\t" + patternGenCount + "\n" +
        "  Static Blocks Generated:\t" + staticsGenCount + "\n" +
        "  Spawn Platforms Generated:\t" + spawnPlatformsGenCount + "\n" +
        "    Total Blocks Generated:\t" + blocks.Count + "\n" +
        "---------------------");
    }

    public void DestroyBlock(Block block, bool animate)
    {
        blocks.Remove(block);
        if (animate)
        {
            Collider2D[] colliders = block.GetComponentsInChildren<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                col.enabled = false;
            }
            block.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            block.StartDestructionAnimation();
            Destroy(block.gameObject, blockDeathTime);
        }
        else
        {
            Destroy(block.gameObject);
        }
    }

    public void DestroyAllBlocks(bool animate)
    {
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            Block block = blocks[i];
            DestroyBlock(block, animate);
        }
    }

    public void StartAppearanceOfAllBlocks()
    {
        foreach (Block block in blocks)
        {
            block.StartAppearanceAnimation();
        }
    }

}
