﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFadeOut : Task {

    private GameObject block;
    private float timeElapsed;
    private float duration;
    private Vector3 initialScale;
    private SpriteRenderer sr;

	public BlockFadeOut(GameObject blk, float dur)
    {
        block = blk;
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        initialScale = block.transform.localScale;
        sr = block.GetComponent<SpriteRenderer>();
    }

    internal override void Update()
    {
        if (block != null)
        {
            timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

            block.transform.localScale = Vector3.LerpUnclamped(initialScale, Vector3.zero, Easing.BackEaseIn(timeElapsed / duration));

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
