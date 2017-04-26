using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlock1 : Block {

	public int blockNum;

	protected override void Init()
	{
		base.Init();
	}

	// Use this for initialization
	void Start () {
		blockNum = 1;
		GetComponent<SpriteRenderer>().color = Services.GameManager.playerColors[blockNum-1];
	}

	protected override void OnCollideWithPlayer(Collision2D collision)
	{
		BlockShift shift = new BlockShift(this, collision.gameObject.GetComponent<Player>().previousVelocity * shiftFactor, shiftDuration);
		Services.TaskManager.AddTask(shift);
	}

	public override void OnBumpedByPlayer(Player player)
	{
		if (player.playerNum == blockNum)
		{
			base.OnBumpedByPlayer(player);
			Services.GameManager.setReady1();
		}
	}

	public override void StartDestructionAnimation(bool playSound)
	{
		base.StartDestructionAnimation(playSound);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
