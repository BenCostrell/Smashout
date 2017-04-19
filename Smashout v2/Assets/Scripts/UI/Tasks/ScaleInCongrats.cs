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
            Services.GameManager.blueTrack++;
            if (Services.GameManager.blueTrack == (Services.GameManager.matchSet / 2 + 1))
            {
                Services.MusicManager.FadeOutTrack();
                Services.GameManager.won = true;
                if (Services.GameManager.greenTrack == 0)
                {
                    congrats.gameObject.GetComponent<Text>().text = "PERFECT MATCH: PURPLE";
                }
                else
                {
                    congrats.gameObject.GetComponent<Text>().text = "MATCH SET: PURPLE";
                }
            }
            else
            {
                congrats.gameObject.GetComponent<Text>().text = "ROUND " + Services.GameManager.round + ": PURPLE";
            }
        }
        else
        {
            Services.GameManager.greenTrack++;
            if (Services.GameManager.greenTrack == (Services.GameManager.matchSet / 2 + 1))
            {
                Services.MusicManager.FadeOutTrack();
                Services.GameManager.won = true;
                if (Services.GameManager.blueTrack == 0)
                {
                    congrats.gameObject.GetComponent<Text>().text = "PERFECT MATCH: GREEN";
                }
                else
                {
                    congrats.gameObject.GetComponent<Text>().text = "MATCH SET: GREEN";
                }
            }
            else
            {
                congrats.gameObject.GetComponent<Text>().text = "ROUND " + Services.GameManager.round + ": GREEN";
            }
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
