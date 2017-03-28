using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleInTitle : Task {
    private RectTransform title;
    private float timeElapsed;
    private float duration;

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.UIManager.titleScaleInTime;
        title = Services.UIManager.title.GetComponent<RectTransform>();
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        title.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.one, Easing.ExpoEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

}
