using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleInTitle : Task {
    private RectTransform title;
    private float timeElapsed;
    private float duration;

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.UIManager.titleScaleInTime;
        title = Services.UIManager.title.GetComponent<RectTransform>();
        title.gameObject.SetActive(true);
    }

    internal override void Update()
    {
        timeElapsed = Mathf.Min(timeElapsed + Time.deltaTime, duration);

        title.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, Easing.QuadEaseOut(timeElapsed / duration));

        if (timeElapsed == duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
