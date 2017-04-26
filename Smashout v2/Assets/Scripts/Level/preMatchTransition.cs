using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preMatchTransition : Task
{
	private float timeElapsed;
	private float duration;

	public preMatchTransition(float dur)
	{
		duration = dur;
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
	}

	protected override void OnSuccess()
	{
		Services.GameManager.StartGame();
		//Services.GameManager.gameStarted = true;
	}
}
