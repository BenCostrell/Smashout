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
        player.SetTrailColor(true);
		player.SetFireColor(true);
        player.SetFireGlow(true);
        player.dashing = true;
    }

    internal override void Update()
    {
        if (player == null) return;
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= activeDuration)
        {
            SetStatus(TaskStatus.Success);
        }

    }

    void SetBumpInactive()
    {
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
        player.GetComponentInChildren<Bumper>().setActiveStatus(false);
        player.rb.gravityScale = player.defaultGravity;
    }

    void CancelDash(ButtonPressed e)
    {
        if (player == null) return;
        if (e.button == "A")
        {
            if (e.playerNum == player.playerNum)
            {
                Vector2 aimVel = new Vector2(Input.GetAxis("Horizontal_P" + e.playerNum), Input.GetAxis("Vertical_P" + e.playerNum));
                Vector2 prevVel = player.rb.velocity;
                //only cancel the dash if the player is not aiming the joystick or is aiming it in a direction that goes against the dash velocity vector
                if (Vector2.Dot(aimVel, prevVel) <= 0)
                {
                    player.rb.velocity = new Vector2(prevVel.x * 0f, prevVel.y);
                    SetStatus(TaskStatus.Aborted);
                }
            }
        }
    }

    void OnBumpHit(BumpHit e)
    {
        if (player == null) return;
        if (e.player == player)
        {
            SetStatus(TaskStatus.Aborted);
        }
    }

    void OnGameOver(GameOver e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    protected override void OnSuccess()
    {
        
    }

    protected override void CleanUp()
    {
        SetBumpInactive();
        if (player == null) return;
        Vector2 prevVel = player.rb.velocity;
        player.rb.velocity = new Vector2(prevVel.x * 0.6f, prevVel.y);
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        Services.EventManager.Unregister<ButtonPressed>(CancelDash);
        player.SetTrailColor(false);
		player.SetFireColor(false);
        player.SetFireGlow(false);
        player.dashing = false;
        player.SetTrailActiveStatus(false);
		player.SetFireActiveStatus (false);
    }
}
