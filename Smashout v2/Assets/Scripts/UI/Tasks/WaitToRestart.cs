using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitToRestart : Task {

    private RectTransform restartPrompt;
    protected override void Init()
    {
        restartPrompt = Services.UIManager.restartPrompt.GetComponent<RectTransform>();
        Services.EventManager.Register<ButtonPressed>(Restart);
        restartPrompt.gameObject.SetActive(true);
        if (Services.GameManager.won == true)
        {
            restartPrompt.gameObject.GetComponent<Text>().text = "Press A to return";
        }
        else
        {
            restartPrompt.gameObject.GetComponent<Text>().text = "Press A to continue";
        }
    }

    void Restart(ButtonPressed e)
    {
        if (e.button == "A")
        {
            SetStatus(TaskStatus.Success);
        }
    }

    protected override void OnSuccess()
    {
        Services.GameManager.NextLevel();
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<ButtonPressed>(Restart);
    }
}
