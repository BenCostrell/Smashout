using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAppear : Task { 
    private float timeElapsed;
    private float duration;
    private List<Vector3> baseScale;
    private List<GameObject> blockParts;
    private Block block;

    public BlockAppear(GameObject blk, float dur)
    {
        block = blk.GetComponent<Block>();
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        baseScale = new List<Vector3>();
        blockParts = new List<GameObject>();
        foreach (SpriteRenderer s in block.GetComponentsInChildren<SpriteRenderer>())
        {
            baseScale.Add(s.transform.localScale);
            blockParts.Add(s.gameObject);
        }
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

        for (int i = 0; i < baseScale.Count; ++i)
        {
            if (!block.GetComponent<BorderBlock>()) blockParts[i].transform.localScale = Vector3.LerpUnclamped(Vector3.zero, baseScale[i], Easing.BackEaseOut(timeElapsed / duration));
        }

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }


}
