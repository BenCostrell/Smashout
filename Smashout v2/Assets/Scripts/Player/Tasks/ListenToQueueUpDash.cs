using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ListenToQueueUpDash : Task
{
    private float timeElapsed;
    private float duration;
    private Player player;
    private bool dashQueued;

    public ListenToQueueUpDash(Player pl, float dur)
    {
        player = pl;
        duration = dur;
    }

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(QueueUpDash);
        timeElapsed = 0;
    }

    void QueueUpDash(ButtonPressed e)
    {
        if (e.playerNum == player.playerNum && e.button == "A")
        {
            dashQueued = true;
        }
    }

    internal override void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.EventManager.Unregister<ButtonPressed>(QueueUpDash);
        if (dashQueued) player.Bump();
    }
}
