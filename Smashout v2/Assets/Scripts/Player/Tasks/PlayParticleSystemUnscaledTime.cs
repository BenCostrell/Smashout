using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayParticleSystemUnscaledTime : Task
{
    private float duration;
    private float timeElapsed;
    private ParticleSystem ps;

    public PlayParticleSystemUnscaledTime(ParticleSystem particleSys)
    {
        ps = particleSys;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        duration = ps.main.duration + ps.main.startLifetime.constant;
    }

    internal override void Update()
    {
        timeElapsed += Time.unscaledDeltaTime;

        ps.Simulate(Time.unscaledDeltaTime, true, false);

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        GameObject.Destroy(ps.gameObject);
    }
}
