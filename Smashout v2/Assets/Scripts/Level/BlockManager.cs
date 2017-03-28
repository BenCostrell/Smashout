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
    public double blockCount;
    public double patternCount;
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

    void Start()
    {
        //Defaults _init and _behaviour if they have not already been set
        init = _init;
        behaviour = _behaviour;

        preInitRNG = UnityEngine.Random.state;
        _init();
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
        DestroyAllBlocks();
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

    Block Create(Vector3 location, int blockType = -1)
    {
        if(blockType == -1)
        {
            UnityEngine.Random.State oldState = UnityEngine.Random.state;
            blockType = UnityEngine.Random.Range(0, blockTypes.Count);
            UnityEngine.Random.state = oldState;
        }
        GameObject obj = Instantiate(blockTypes[blockType], location, Quaternion.identity) as GameObject;
        Block block = obj.GetComponent<Block>();
        block.GetComponent<SpriteRenderer>().enabled = false;
        return block;
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
        blocks = new List<Block>();
        Block block;
        for (int i = 0; i < blockCount; i++)
        {
            block = GenerateValidBlock();
            if (block.transform.position == Vector3.forward)
            {
                Destroy(block.gameObject);
                Debug.Log("only made " + i + "blocks");
                break;
            }
            else
            {
                blocks.Add(block);
            }
        }
    }

    public void DestroyBlock(Block block)
    {
        Collider2D[] colliders = block.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
        block.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        block.StartDestructionAnimation();
        blocks.Remove(block);
        Destroy(block.gameObject, blockDeathTime);
    }

    public void DestroyAllBlocks()
    {
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            Block block = blocks[i];
            DestroyBlock(block);
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
