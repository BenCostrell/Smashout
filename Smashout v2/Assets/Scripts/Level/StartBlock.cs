using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlock : Block
{

    public int blockNum;

    protected override void Init()
    {
        base.Init();
    }

    // Use this for initialization
    void Start()
    {
        if(transform.position.x < 0)
        {
            blockNum = 1;
        }
        else
        {
            blockNum = 2;
        }
        GetComponent<SpriteRenderer>().color = Services.GameManager.playerColors[blockNum - 1];
        GetComponentInChildren<Light>().color = Services.GameManager.playerColors[blockNum - 1];
        ParticleSystem.MainModule psm = GetComponentInChildren<ParticleSystem>().main;
        psm.startColor = Services.GameManager.playerColors[blockNum - 1];
    }

    protected override void OnCollideWithPlayer(Collision2D collision)
    {
        BlockShift shift = new BlockShift(this, collision.gameObject.GetComponent<Player>().previousVelocity * shiftFactor, shiftDuration);
        Services.TaskManager.AddTask(shift);
    }

    public override void OnBumpedByPlayer(Player player)
    {
        if (player.playerNum == blockNum)
        {
            base.OnBumpedByPlayer(player);
            Services.GameManager.setReady(blockNum);
        }
    }

    public override void StartDestructionAnimation(bool playSound)
    {
        base.StartDestructionAnimation(playSound);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
