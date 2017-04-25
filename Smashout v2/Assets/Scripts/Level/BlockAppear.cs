using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAppear : Task { 
    private float timeElapsed;
    private float duration;
    private Vector3 baseScale;
    private GameObject block;

    public BlockAppear(GameObject blk, float dur)
    {
        block = blk;
        duration = dur;
    }

    protected override void Init()
    {
        block.GetComponent<SpriteRenderer>().enabled = true;
        timeElapsed = 0;
        baseScale = block.transform.localScale;
		block.transform.localScale = Vector3.zero;
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

		if (block.tag != "Border") block.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, baseScale, Easing.BackEaseOut(timeElapsed / duration));

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }


}
