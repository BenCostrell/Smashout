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
        player.UnlockAllInput();
    }
}
