using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PowerUpAnimation: Task
{
    private float duration;
    private float timeElapsed;
    private Player player;

    public PowerUpAnimation(Player pl, float dur)
    {
        player = pl;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        player.GetComponent<SpriteRenderer>().color = Color.white;
        //Time.timeScale = 0.1f;
        //duration = duration * Time.timeScale;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        player.GetComponent<SpriteRenderer>().color = Services.GameManager.playerColors[player.playerNum - 1];
        //Time.timeScale = 1;
    }
}
