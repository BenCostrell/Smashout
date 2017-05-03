using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SetTutorialText : Task
{
    private TextMesh tutorialText;
    private Vector3 originalSize;
    private string newText;
    float duration, wait, elapsed;
    Easing.Function ease;

    public SetTutorialText(string text, TextMesh tutorialTxt, float dura, float wait, Easing.Function easingFunc)
    {
        newText = text;
        tutorialText = tutorialTxt;
        duration = dura;
        ease = easingFunc;
        originalSize = tutorialText.transform.localScale;
    }

    protected override void Init()
    {
        elapsed = 0;
    }

    internal override void Update()
    {
        if (!tutorialText)
        {
            SetStatus(TaskStatus.Aborted);
            return;
        }
        elapsed += Time.deltaTime;
        if (elapsed < duration / 2) tutorialText.transform.localScale = Vector3.Lerp(originalSize, Vector3.zero, ease(elapsed / (duration / 2)));
        else if (elapsed < duration)
        {
            Debug.Log(tutorialText.text);
            Debug.Log(originalSize);
            tutorialText.text = newText;
            tutorialText.transform.localScale = Vector3.Lerp(Vector3.zero, originalSize, ease((elapsed - (duration / 2)) / (duration / 2)));
        }
        else if (elapsed > duration + wait) SetStatus(TaskStatus.Success);
    }
}
