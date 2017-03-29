using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpTask : LockOutButtonInput {

    public BumpTask(float dur, Player pl) : base(dur, pl) { }

    protected override void Init()
    {
        base.Init();
        player.gameObject.GetComponent<SpriteRenderer>().color = Services.GameManager.bumpColors[player.playerNum-1];
        player.bump = true;
        player.GetComponentInChildren<Bumper>().setActiveStatus(true);
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
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
        player.bump = false;
        player.GetComponentInChildren<Bumper>().setActiveStatus(false);
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
    }
}
