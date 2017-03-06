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
	public float blockBounceFactor;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		bumper = GetComponentInChildren<Bumper> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate(){
		ProcessInput ();
	}

	void ProcessInput(){
		float x = Input.GetAxis ("Horizontal_P" + playerNum);
		Move (x);
		if (Input.GetButtonDown ("Bump_P" + playerNum)) {
			Bump ();
		}
	}

	void Move(float x){
		Vector2 forceVector = new Vector2 (x * driftForce, 0);
		rb.AddForce (forceVector);
	}

	void Bump(){
		bumper.Activate ();
	}

	public void GetBumped(Vector2 bumpVector){
		rb.velocity += bumpVector;
	}

	bool IsGrounded(){
		return Physics2D.Raycast (transform.position, Vector2.down, groundDetectionDistance, groundLayer);
	}

	void OnCollisionEnter2D(Collision2D collision){
		GameObject obj = collision.collider.gameObject;
		if (obj.tag == "Surface") {
			Vector3 launchVector = obj.GetComponent<Surface>().surfaceNormal * blockBounceFactor;
			GetBumped (launchVector);
		}
	}
}
