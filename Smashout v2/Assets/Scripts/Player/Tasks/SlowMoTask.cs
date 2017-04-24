using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SlowMoTask : Task          
{
    private float scale;
    private float duration;
    private float timeElapsed;
    private float trackSlowMoTime;
    public SlowMoTask(float scl, float dur)
    {
        scale = scl;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        trackSlowMoTime = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;
        trackSlowMoTime += Time.deltaTime;

        if (timeElapsed <= duration / 2)
        {
            Time.timeScale = Mathf.Lerp(1, scale, Easing.ExpoEaseOut(timeElapsed / (duration / 2)));
        }
        else
        {
            Time.timeScale = Mathf.Lerp(scale, 1, Easing.ExpoEaseIn((timeElapsed-(duration/2)) / (duration / 2)));
        }

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Time.timeScale = 1;
    }
}
