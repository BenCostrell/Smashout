using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitToRestart : Task {

    protected override void Init()
    {
        Services.EventManager.Register<ButtonPressed>(Restart);
        Services.UIManager.restartPrompt.SetActive(true);
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
