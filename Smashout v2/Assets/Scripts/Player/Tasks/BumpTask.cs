using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpTask : Task {
    private float activeDuration;
    private Player player;
    private float timeElapsed;

    public BumpTask(Player pl, float activeDur)
    {
        player = pl;
        activeDuration = activeDur;
    }

    protected override void Init()
    {
        if (player == null) return;
        timeElapsed = 0;
        player.rb.gravityScale = 0;
        player.gameObject.GetComponent<SpriteRenderer>().color = Services.GameManager.bumpColors[player.playerNum-1];
        player.GetComponentInChildren<Bumper>().setActiveStatus(true);
        Services.EventManager.Register<BumpHit>(OnBumpHit);
        Services.EventManager.Register<GameOver>(OnGameOver);
        player.SetTrailStatus(true);
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= activeDuration)
        {
            SetStatus(TaskStatus.Success);
        }

    }

    void SetBumpInactive()
    {
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
        player.GetComponentInChildren<Bumper>().setActiveStatus(false);
        player.rb.gravityScale = player.defaultGravity;
    }

    void OnBumpHit(BumpHit e)
    {
        if (e.player == player)
        {
            SetStatus(TaskStatus.Aborted);
        }
    }

    void OnGameOver(GameOver e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    protected override void CleanUp()
    {
        SetBumpInactive();
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        player.SetTrailStatus(false);
    }
}
