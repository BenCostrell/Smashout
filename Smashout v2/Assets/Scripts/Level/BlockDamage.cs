using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDamage : Task {

    private Block block;
    private float timeElapsed;
    private float duration;
    private int state;

	public BlockDamage(Block blk, float dur)
    {
        block = blk;
        duration = dur;
        state = blk.damage;
    }

    protected override void Init()
    {
        timeElapsed = 0;
    }

    internal override void Update()
    {
        if (block != null)
        {
            timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);
            foreach(SpriteRenderer sr in block.damageStates[state-1].GetComponentsInChildren<SpriteRenderer>())
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(sr.color.a, 0, Easing.QuadEaseInOut(timeElapsed / duration)));
            }
            foreach (Light l in block.damageStates[state - 1].GetComponentsInChildren<Light>())
            {
                l.intensity = Mathf.Lerp(l.intensity, 0, Easing.QuadEaseInOut(timeElapsed / duration));
            }

            if (timeElapsed == duration)
            {
                SetStatus(TaskStatus.Success);
            }
        }
        else
        {
            SetStatus(TaskStatus.Aborted);
        }
    }

    protected override void OnSuccess()
    {
    }
}
