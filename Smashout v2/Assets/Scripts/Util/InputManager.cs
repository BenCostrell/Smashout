using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager {
    public void GetInput()
    {
        if (Input.GetButtonDown("A_P1"))
        {
            Services.EventManager.Fire(new ButtonPressed("A", 1));
        }
        if (Input.GetButtonDown("A_P2"))
        {
            Services.EventManager.Fire(new ButtonPressed("A", 2));
        }
        if (Input.GetButtonDown("Reset"))
        {
            Services.EventManager.Fire(new Reset());
        }
    }
}
