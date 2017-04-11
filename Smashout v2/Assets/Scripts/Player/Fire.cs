﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour {

	private Player player;

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<Player> ();
		ParticleSystem ps = GetComponent<ParticleSystem> ();
		var col = ps.colorOverLifetime;
		col.enabled = true;

		col.color = player.fireColor;
	}
	
	// Update is called once per frame
	void Update () {
		//this.gameObject.GetComponents<Transform>(). = Mathf.Atan2 (player.rb.velocity.x, player.rb.velocity.y) * 180 / Mathf.PI;
		//transform.rotation 
	}
}