using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleInCongrats : Task {

    private RectTransform congrats;
    private float timeElapsed;
    private float duration;
    private int winningPlayer;

    public ScaleInCongrats(int winner)
    {
        winningPlayer = winner;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        duration = Services.UIManager.congratsScaleInTime;
        congrats = Services.UIManager.congrats.GetComponent<RectTransform>();
        congrats.gameObject.SetActive(true);
        congrats.gameObject.GetComponent<Text>().color = Services.GameManager.playerColors[winningPlayer - 1];
        if (winningPlayer == 1)
        {
            congrats.gameObject.GetComponent<Text>().text = "BLUE WINS";
        }
        else
        {
            congrats.gameObject.GetComponent<Text>().text = "GREEN WINS";
        }
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        congrats.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, Easing.QuadEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }
}
