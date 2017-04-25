using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlock2 : Block {

	public int blockNum;

	// Use this for initialization
	void Start () {
		blockNum = 2;
		GetComponent<SpriteRenderer>().color = Services.GameManager.playerColors[blockNum-1];
	}

	protected override void OnCollideWithPlayer(Collision2D collision)
	{
		base.OnCollideWithPlayer(collision);
	}

	public override void OnBumpedByPlayer(Player player)
	{
		if (player.playerNum == blockNum)
		{
			base.OnBumpedByPlayer(player);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
