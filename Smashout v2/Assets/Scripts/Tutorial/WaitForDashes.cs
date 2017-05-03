using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WaitForDashes : Task
{
    private int[] playerDashes;
    private int numRequiredDashes;

    public WaitForDashes(int numReqDashes)
    {
        numRequiredDashes = numReqDashes;
    }


    protected override void Init()
    {
        playerDashes = new int[Services.GameManager.numPlayers];
        for (int i = 0; i < playerDashes.Length; i++)
        {
            playerDashes[i] = 0;
        }
        Services.EventManager.Register<ButtonPressed>(PlayerPressedButton);
    }

    void PlayerPressedButton(ButtonPressed e)
    {
        if (e.button == "A")
        {
            playerDashes[e.playerNum - 1] += 1;
            bool ready = true;
            foreach (int numDashes in playerDashes)
            {
                if (numDashes < numRequiredDashes) ready = false;
            }
            if (ready) SetStatus(TaskStatus.Success);
        }
    }

    protected override void CleanUp()
    {
        Services.EventManager.Unregister<ButtonPressed>(PlayerPressedButton);
    }
}
