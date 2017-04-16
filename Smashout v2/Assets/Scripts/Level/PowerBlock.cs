using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBlock : Block {

    protected override void Init()
    {
        base.Init();
    }
    protected override void OnCollideWithPlayer(Collision2D collision)
    {
        base.OnCollideWithPlayer(collision);
        collision.gameObject.GetComponent<Player>().power += 1;
    }

    public override void OnBumpedByPlayer(Player player)
    {
        player.power += 1;
        base.OnBumpedByPlayer(player);
    }

    public override void StartDestructionAnimation(bool playSound)
    {
        base.StartDestructionAnimation(playSound);
    }
}
