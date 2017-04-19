using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BlockShift : Task
{
    private Block block;
    private Vector3 shift;
    private float duration;
    private float timeElapsed;
    private Vector3 initialPosition;

    public BlockShift(Block blk, Vector3 shft, float dur)
    {
        block = blk;
        shift = shft;
        duration = dur;
    }

    protected override void Init()
    {
        initialPosition = block.transform.position;
        timeElapsed = 0;
    }

    internal override void Update()
    {
        if (block != null)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed <= duration / 2)
            {
                block.transform.position = Vector3.Lerp(initialPosition, initialPosition + shift,
                    Easing.QuadEaseOut(timeElapsed / (duration / 2)));
            }
            else
            {
                block.transform.position = Vector3.Lerp(initialPosition + shift, initialPosition,
                    Easing.QuadEaseIn((timeElapsed - (duration / 2)) / (duration / 2)));
            }

            if (timeElapsed >= duration)
            {
                SetStatus(TaskStatus.Success);
            }
        }
        else
        {
            SetStatus(TaskStatus.Aborted);
        }
    }
}
