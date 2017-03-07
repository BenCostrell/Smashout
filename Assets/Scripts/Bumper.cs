using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

	private SpriteRenderer sr;
	private CircleCollider2D col;
	public float duration;
	private Player player;
	public float blockLaunchPower;
	private bool bumped;
	public bool isActive;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		col = GetComponent<CircleCollider2D> ();
		player = GetComponentInParent<Player> ();
		bumped = false;

		Deactivate ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetActiveStatus(bool active){
		if (active) {
			sr.enabled = true;
			col.enabled = true;
			isActive = true;
		} else {
			sr.enabled = false;
			col.enabled = false;
			isActive = false;
		}
	}

	public void Activate(){
		SetActiveStatus (true);
		Invoke ("Deactivate", duration);
	}

	void Deactivate(){
		SetActiveStatus (false);
		bumped = false;
	}

	void OnTriggerEnter2D(Collider2D collider){
		GameObject obj = collider.gameObject;
		if (obj.tag == "Surface" && !bumped) {
			Vector3 launchVector = obj.GetComponent<Surface>().surfaceNormal * blockLaunchPower;
			player.GetBumped (launchVector, true);
			bumped = true;
			obj.transform.parent.gameObject.GetComponent<Block> ().DestroyThis ();
		}
	}
}
