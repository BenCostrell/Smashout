using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOutAllInput : Task {

    private float duration;
    private float timeElapsed;
    protected Player player;

    public LockOutAllInput(float dur, Player pl)
    {
        duration = dur;
        player = pl;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        player.LockAllInput();
        Services.EventManager.Fire(new PlayerLockedOut(player));
        Services.EventManager.Register<PlayerLockedOut>(Interrupt);
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    void Interrupt(PlayerLockedOut e)
    {
        if (e.player == player)
        {
            SetStatus(TaskStatus.Aborted);
        }
    }

    protected override void OnSuccess()
    {
        player.UnlockAllInput();
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<PlayerLockedOut>(Interrupt);
    }
}
