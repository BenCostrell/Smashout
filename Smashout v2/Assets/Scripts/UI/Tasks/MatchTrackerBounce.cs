using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MatchTrackerBounce :Task
{
    private float duration;
    private float timeElapsed;
    private RectTransform trackerCircle;
    private Vector2 baseSize;
    private Vector3 pos;
    private float shakeFactor;

    public MatchTrackerBounce(RectTransform circle, float dur)
    {
        trackerCircle = circle;
        pos = trackerCircle.position;
        duration = dur;
        shakeFactor = 10.0f;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        baseSize = trackerCircle.localScale;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed <= duration / 2)
        {
            trackerCircle.localScale = Vector2.Lerp(baseSize, 1.8f * baseSize, Easing.QuadEaseOut(timeElapsed / (duration / 2)));
        }
        else
        {
            trackerCircle.localScale = Vector2.Lerp(1.8f * baseSize, baseSize, Easing.QuadEaseIn((timeElapsed - (duration / 2)) / (duration / 2)));
        }

        trackerCircle.position = new Vector3(trackerCircle.position.x + shakeFactor, trackerCircle.position.y, trackerCircle.position.z);
        shakeFactor *= -1;
        if (timeElapsed >= duration)
        {
            trackerCircle.position = pos;
            SetStatus(TaskStatus.Success);
        }
    }
}
