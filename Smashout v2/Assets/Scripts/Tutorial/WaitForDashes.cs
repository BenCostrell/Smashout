using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WaitForDashes : Task
{
    private bool[] playersReady;

    protected override void Init()
    {
        playersReady = new bool[Services.GameManager.numPlayers];
        for (int i = 0; i < playersReady.Length; i++)
        {
            playersReady[i] = false;
        }
        Services.EventManager.Register<ButtonPressed>(PlayerPressedButton);
    }

    void PlayerPressedButton(ButtonPressed e)
    {
        playersReady[e.playerNum - 1] = true;
        bool ready = true;
        foreach(bool playerReady in playersReady)
        {
            if (!playerReady) ready = false;
        }
        if (ready) SetStatus(TaskStatus.Success);
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<ButtonPressed>(PlayerPressedButton);
    }
}
