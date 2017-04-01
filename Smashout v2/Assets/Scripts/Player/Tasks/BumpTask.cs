using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpTask : LockOutButtonInput {
    private float activeDuration;

    public BumpTask(float dur, Player pl, float activeDur) : base(dur, pl)
    {
        activeDuration = activeDur;
    }

    protected override void Init()
    {
        base.Init();
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = Services.GameManager.bumpColors[player.playerNum-1];
        player.bump = true;
        player.GetComponentInChildren<Bumper>().setActiveStatus(true);
        Services.EventManager.Register<BumpHit>(OnBumpHit);
        Services.EventManager.Register<GameOver>(OnGameOver);
        //player.SetTrailStatus(true);
    }

    internal override void Update()
    {
        base.Update();

        if (timeElapsed >= activeDuration)
        {
            SetBumpInactive();
        }
    }

    void SetBumpInactive()
    {
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
        player.bump = false;
        player.GetComponentInChildren<Bumper>().setActiveStatus(false);
    }

    void OnBumpHit(BumpHit e)
    {
        if (e.player == player)
        {
            SetBumpInactive();
        }
    }

    void OnGameOver(GameOver e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        if (player == null) return;
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        //player.SetTrailStatus(false);
    }
}
