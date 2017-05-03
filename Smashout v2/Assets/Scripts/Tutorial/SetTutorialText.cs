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

    public SetTutorialText(string text, TextMesh tutorialTxt)
    {
        newText = text;
        tutorialText = tutorialTxt;
    }

    protected override void Init()
    {
        tutorialText.text = newText;
        SetStatus(TaskStatus.Success);
    }
}
