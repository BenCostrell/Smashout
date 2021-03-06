﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOutButtonInput : Task {

    protected float duration;
    protected float timeElapsed;
    protected Player player;

    public LockOutButtonInput(float dur, Player pl)
    {
        duration = dur;
        player = pl;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        player.LockButtonInput();
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        player.UnlockButtonInput();
    }
}
