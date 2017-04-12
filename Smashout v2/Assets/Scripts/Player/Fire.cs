﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour {

	private Player player;

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<Player> ();
		changeColor (player.fireColor);
	}
	
	// Update is called once per frame
	void Update () {
		//this.gameObject.GetComponents<Transform>(). = Mathf.Atan2 (player.rb.velocity.x, player.rb.velocity.y) * 180 / Mathf.PI;
		//transform.rotation 
	}

	public void changeColor(Gradient newColor) {
		ParticleSystem ps = GetComponent<ParticleSystem> ();
		var col = ps.colorOverLifetime;

		col.color = newColor;
	}

	public void controlFlow(bool on) {
		ParticleSystem ps = GetComponent<ParticleSystem> ();
		if (on) {
			ps.Play();
		} else {
			ps.Stop();
		}
	}
}
