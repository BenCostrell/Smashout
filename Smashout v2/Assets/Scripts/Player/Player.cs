using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int playerNum;
    public float bumpCooldown;
    private bool actionable;

    // Use this for initialization
    void Start()
    {
        UnlockAllInput();
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (actionable)
        {
            Move();
        }
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
        LockOutButtonInput bumpCooldownTask = new LockOutButtonInput(bumpCooldown, this);
        Services.TaskManager.AddTask(bumpCooldownTask);
    }

    public void GetStunned(float stunDuration)
    {
        LockOutAllInput hitstunTask = new LockOutAllInput(stunDuration, this);
        Services.TaskManager.AddTask(hitstunTask);
    }

    public void LockButtonInput()
    {
        Services.EventManager.Unregister<ButtonPressed>(OnButtonPressed);
    }

    public void LockAllInput()
    {
        LockButtonInput();
        actionable = false;
    }

    public void UnlockButtonInput()
    {
        Services.EventManager.Register<ButtonPressed>(OnButtonPressed);
    }

    public void UnlockAllInput()
    {
        UnlockButtonInput();
        actionable = true;
    }

    void Die()
    {
        LockAllInput();
        Services.EventManager.Fire(new GameOver(playerNum));
    }

    void OnGameOver(GameOver e)
    {
    }
}
