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
    private List<Vector3> initialPositions;
    private List<GameObject> objectsToShift;

    public BlockShift(Block blk, Vector3 shft, float dur)
    {
        block = blk;
        shift = shft;
        duration = dur;
    }

    protected override void Init()
    {
        SpriteRenderer[] spriteRenderers = block.GetComponentsInChildren<SpriteRenderer>();
        initialPositions = new List<Vector3>();
        objectsToShift = new List<GameObject>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr.gameObject != block.gameObject)
            {
                objectsToShift.Add(sr.gameObject);
                initialPositions.Add(sr.transform.position);
            }
        }
        timeElapsed = 0;
    }

    internal override void Update()
    {
        if (block != null)
        {
            timeElapsed += Time.deltaTime;

            for (int i = 0; i < objectsToShift.Count; i++)
            {
                if (timeElapsed <= duration / 2)
                {
                    objectsToShift[i].transform.position = Vector3.Lerp(initialPositions[i], initialPositions[i] + shift,
                        Easing.QuadEaseOut(timeElapsed / (duration / 2)));
                }
                else
                {
                    objectsToShift[i].transform.position = Vector3.Lerp(initialPositions[i] + shift, initialPositions[i],
                        Easing.QuadEaseIn((timeElapsed - (duration / 2)) / (duration / 2)));
                } 
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
