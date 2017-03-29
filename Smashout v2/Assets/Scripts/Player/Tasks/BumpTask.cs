using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpTask : LockOutButtonInput {

    public BumpTask(float dur, Player pl) : base(dur, pl) { }

    protected override void Init()
    {
        base.Init();
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = Services.GameManager.bumpColors[player.playerNum-1];
        player.bump = true;
        Services.EventManager.Register<BumpHit>(OnBumpHit);

    }

    void OnBumpHit(BumpHit e)
    {
        if (e.player == player)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
        player.bump = false;
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
    }
}
