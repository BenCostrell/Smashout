using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public int playerNum;
	public float groundDetectionDistance;
	public LayerMask groundLayer;
	private Rigidbody2D rb;
	public float driftForce;
	private Bumper bumper;
	private Transform bumperCooldownObj;
	public float blockBounceFactor;
	public float bumpCooldown;
	private float bumpCooldownTimer;
	private Vector2 previousVelocity;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		bumper = GetComponentInChildren<Bumper> ();
		bumpCooldownTimer = 0;
		bumperCooldownObj = GetComponentsInChildren<Transform> () [2];
	}
	
	// Update is called once per frame
	void Update () {
		previousVelocity = rb.velocity;
	}

	void FixedUpdate(){
		ProcessInput ();
	}

	void ProcessInput(){
		float x = Input.GetAxis ("Horizontal_P" + playerNum);
		Move (x);
		if (bumpCooldownTimer > 0) {
			bumpCooldownTimer -= Time.deltaTime;
			UpdateCooldownTimerObj ();
		} else {
			if (Input.GetButton ("Bump_P" + playerNum) && !bumper.isActive) {
				Bump ();
			}
		}
	}

	void Move(float x){
		Vector2 forceVector = new Vector2 (x * driftForce, 0);
		rb.AddForce (forceVector);
	}

	void Bump(){
		bumper.Activate ();
		bumpCooldownTimer = bumpCooldown;
		UpdateCooldownTimerObj ();
	}

	public void GetBumped(Vector2 bumpVector, bool vertical){
		Vector2 newVelocity;
		if (vertical) {
			newVelocity = new Vector2 (0, bumpVector.y);
		} else {
			newVelocity = new Vector2 (bumpVector.x, 0);
		}

		rb.velocity = newVelocity;
	}

	bool IsGrounded(){
		return Physics2D.Raycast (transform.position, Vector2.down, groundDetectionDistance, groundLayer);
	}

	void OnCollisionEnter2D(Collision2D collision){
		GameObject obj = collision.collider.gameObject;
		if (obj.tag == "Surface") {
			Vector3 launchVector = previousVelocity * blockBounceFactor;
			launchVector = new Vector3 (launchVector.x, -launchVector.y, 0);
			GetBumped (launchVector, true);
		}
	}

	void UpdateCooldownTimerObj(){
		bumperCooldownObj.localScale = Vector3.one * Mathf.Lerp (0, 1, 1 - (bumpCooldownTimer / bumpCooldown));
	}
}
