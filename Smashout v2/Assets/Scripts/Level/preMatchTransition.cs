using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preMatchTransition : Task
{
	private float timeElapsed;
	private float duration;
    private float keepPercent;
    private bool exploded;

	public preMatchTransition(float dur, float keepPer)
	{
		duration = dur;
        keepPercent = keepPer;
        exploded = false;
	}

	protected override void Init ()
	{
		timeElapsed = 0;
	}

	internal override void Update()
	{
		timeElapsed += Time.deltaTime;

		if (timeElapsed >= duration)
		{
			SetStatus (TaskStatus.Success);
		}
        if (!exploded && timeElapsed >= duration / 4)
        {
            foreach (Player p in Services.GameManager.players)
            {
                p.startScreenDie();
            }
            exploded = true;
        }
        //foreach (Player p in Services.GameManager.players)
        //{
        //    p.gameObject.transform.localScale *= keepPercent;
        //}
    }

	protected override void OnSuccess()
	{
        foreach (Player p in Services.GameManager.players)
        {
            Object.Destroy(p.gameObject);
        }
        Services.GameManager.StartGame();
	}
}
