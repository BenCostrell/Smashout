using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SetTutorialText : Task
{
    private TextMesh tutorialText;
    private string newText;
    private float timeElapsed;
    private float duration;

    public SetTutorialText(string text, TextMesh tutorialTxt, float dur)
    {
        newText = text;
        tutorialText = tutorialTxt;
        duration = dur;
    }

    protected override void Init()
    {
        tutorialText.text = newText;
        timeElapsed = 0;
        tutorialText.transform.localScale = Vector3.zero;
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        tutorialText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Easing.QuadEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
