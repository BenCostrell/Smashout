using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preMatchTransition : Task
{
	private float timeElapsed;
	private float duration;
    private float explodeStartPercent;
    private bool exploded;

	public preMatchTransition(float dur, float expPer)
	{
		duration = dur;
        explodeStartPercent = expPer;
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
        if (!exploded && timeElapsed >= duration*explodeStartPercent)
        {
            foreach (Player p in Services.GameManager.players)
            {
                p.startScreenDie();
            }
            exploded = true;
        }
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
