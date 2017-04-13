using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WaitForTime : Task
{
    private float duration;
    private float timeElapsed;

    public WaitForTime(float time)
    {
        duration = time;
    }

    protected override void Init()
    {
        timeElapsed = 0;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

}
