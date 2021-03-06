﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToStart : Task {

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(BeginGame);
        Services.UIManager.startPrompt.SetActive(true);
    }

    void BeginGame(ButtonPressed e)
    {
        if (e.button == "A")
        {
            Services.GameManager.tutorialOn = false;
            SetStatus(TaskStatus.Success);
        }
        else if (e.button == "B")
        {
            Services.GameManager.tutorialOn = true;
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void CleanUp()
    {
        Services.UIManager.startPrompt.SetActive(false);
        Services.UIManager.title.SetActive(false);
        Services.EventManager.Unregister<ButtonPressed>(BeginGame);
    }
}
