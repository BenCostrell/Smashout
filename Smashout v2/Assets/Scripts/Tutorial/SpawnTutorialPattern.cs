using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpawnTutorialPattern : Task
{
    private GameObject pattern;
    private List<Block> blocks;
    private bool waitUntilBlocksDestroyed;

    public SpawnTutorialPattern(GameObject pat, bool waitToDestroy)
    {
        pattern = pat;
        waitUntilBlocksDestroyed = waitToDestroy;
    }

    protected override void Init()
    {
        blocks = new List<Block>();
        GameObject tempPattern = GameObject.Instantiate(pattern, Vector3.zero, Quaternion.identity) as GameObject;
        foreach (Transform b in tempPattern.GetComponentsInChildren<Transform>())
        {
            if (b.gameObject == tempPattern) continue;

            if(b.GetComponent<Block>()) b.parent = null;
            Services.BlockManager.blocks.Add(b.GetComponent<Block>());
            blocks.Add(b.GetComponent<Block>());
        }
        GameObject.Destroy(tempPattern);
        if (!waitUntilBlocksDestroyed) SetStatus(TaskStatus.Success);
    }

    internal override void Update()
    {
        bool done = true;
        foreach(Block block in blocks)
        {
            if (block != null)
            {
                done = false;
                break;
            }
        }
        if (done) SetStatus(TaskStatus.Success);
    }
}
