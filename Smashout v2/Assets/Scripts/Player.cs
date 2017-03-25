using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int playerNum;

    // Use this for initialization
    void Start()
    {
        Services.EventManager.Register<ButtonPressed>(OnButtonPressed);
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {

    }

    void OnButtonPressed(ButtonPressed e)
    {
        string button = e.button;
        if (e.playerNum == playerNum)
        {
            if (button == "A")
            {
                Bump();
            }
        }
    }

    void Bump()
    {

    }

    void Die()
    {
        Services.EventManager.Fire(new GameOver(playerNum));
    }

    void OnGameOver(GameOver e)
    {
    }
}
