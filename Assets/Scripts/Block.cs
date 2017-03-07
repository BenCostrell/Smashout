using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

	private BlockManager blockManager;

	// Use this for initialization
	void Start () {
		blockManager = GameObject.FindGameObjectWithTag ("BlockManager").GetComponent<BlockManager>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void DestroyThis(){
		blockManager.DestroyBlock (this);
	}

	public void StartDestructionAnimation(){
		iTween.ScaleTo (gameObject, iTween.Hash ("scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack, 
			"time", blockManager.blockDeathTime));
	}
}
