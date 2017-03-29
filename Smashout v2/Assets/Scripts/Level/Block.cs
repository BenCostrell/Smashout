using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    int health = 1;

    void Start () {
	}

	void Update () {
        if(health < 1)
        {
            StartDestructionAnimation();
        }
	}

    public void DestroyThis()
    {
        Services.BlockManager.DestroyBlock(this, true);
    }

    public void StartDestructionAnimation()
    {
        BlockFadeOut fadeOut = new BlockFadeOut(gameObject, Services.BlockManager.blockDeathTime);
        Services.TaskManager.AddTask(fadeOut);
    }

    public void StartAppearanceAnimation()
    {
        BlockAppear appear = new BlockAppear(gameObject, Services.BlockManager.blockAppearanceTime);
        Services.TaskManager.AddTask(appear);
    }
}
