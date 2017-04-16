using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DeathBlock : Block
{
    protected override void Init()
    {
        
    }
    protected override void OnCollideWithPlayer(Collision2D collision)
    {
        collision.gameObject.GetComponent<Player>().Die();
    }

    public override void StartAppearanceAnimation()
    {
    }

    public override void StartDestructionAnimation(bool playSound)
    {
    }

    public override void OnBumpedByPlayer(Player player)
    {
    }
}
