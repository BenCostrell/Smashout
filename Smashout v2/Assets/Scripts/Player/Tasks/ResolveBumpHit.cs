using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResolveBumpHit : Task
{
    private Player player;
    private Player enemy;
    public ResolveBumpHit(Player pl, Player en)
    {
        player = pl;
        enemy = en;
    }

    protected override void Init()
    {
        player.ResolveBumpHit(enemy);
        SetStatus(TaskStatus.Success);
    }
}
