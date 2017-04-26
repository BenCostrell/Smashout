using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundTask : Task
{
    private int winningPlayer;
    private Player player;
    private int roundNum;
    private float timeElapsed;
    public float duration;
    public float threshold;
    private ScaleInMatch script;
    private Vector3 point;
    private RectTransform rect;

    public RoundTask(int pl, float d, float t)
    {
        winningPlayer = pl;
        duration = d;
        threshold = t;
    }

    // Use this for initialization 
    protected override void Init()
    {
        timeElapsed = 0;
        script = Services.UIManager.matchCount.GetComponent<ScaleInMatch>();
        player = Services.GameManager.players[winningPlayer - 1];
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        player.GetComponent<CircleCollider2D>().enabled = false;
        if (winningPlayer == 1)
        {
            script.blueRound++;
            Debug.Log("Blue Round: " + script.blueRound);
            roundNum = script.blueRound - 1;
            //Debug.Log("Blue " + Services.GameManager.blueTrack); 
        }
        else if (winningPlayer == 2)
        {
            script.greenRound++;
            Debug.Log("Green Round: " + script.greenRound);
            roundNum = script.roundCircles.Length - 1 - (script.greenRound - 1);
            //Debug.Log("Green " + Services.GameManager.greenTrack); 
        }
        rect = script.roundCircles[roundNum].GetComponent<RectTransform>();
        point = Camera.main.ScreenToWorldPoint(rect.transform.position);
    }

    // Update is called once per frame 
    internal override void Update()
    {
        timeElapsed += Time.deltaTime;
        float step = player.dashSpeed * Time.deltaTime * 2;
        player.transform.position = Vector3.Lerp(player.transform.position, point, Easing.QuadEaseOut(timeElapsed / duration));

        if (Vector3.Distance(player.transform.position, point) < threshold)
        {
            SetStatus(TaskStatus.Success);
        }

    }

    protected override void OnSuccess()
    {
        Color color = Services.GameManager.playerColors[winningPlayer - 1];
        Debug.Log(roundNum);
        player.gameObject.SetActive(false);
        script.markAsWon(roundNum, color);
    }

    protected override void CleanUp()
    {
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        player.GetComponent<CircleCollider2D>().enabled = true;
        rect = null;
        winningPlayer = 0;
        duration = 0;
        player = null;
        roundNum = 0;
        script = null;
        point = new Vector3(0, 0, 0);
        timeElapsed = 0;
    }
}