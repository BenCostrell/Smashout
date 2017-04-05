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
        Services.EventManager.Register<ButtonPressed>(CancelDash);
        Services.EventManager.Register<GameOver>(OnGameOver);
        player.SetTrailStatus(true);
        player.dashing = true;
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

    void CancelDash(ButtonPressed e)
    {
        if (e.playerNum == player.playerNum)
        {
            SetStatus(TaskStatus.Aborted);
        }
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
        Vector2 prevVel = player.rb.velocity;
        player.rb.velocity = new Vector2(prevVel.x * 0.6f, prevVel.y);
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        Services.EventManager.Unregister<ButtonPressed>(CancelDash);
        player.SetTrailStatus(false);
        player.dashing = false;
    }
}
