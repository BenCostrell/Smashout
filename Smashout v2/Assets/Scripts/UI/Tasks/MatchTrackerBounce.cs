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

    public MatchTrackerBounce(RectTransform circle, float dur)
    {
        trackerCircle = circle;
        duration = dur;
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

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
