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
	public float playerBounceFactor;
	public float bumpCooldown;
	private float bumpCooldownTimer;
	private Vector2 previousVelocity;
	public float hitstunTimer;
	public float hitstunDuration;
	public Color defaultColor;
	private int comboCounter;
	public float comboScaling;
	public float platformLifetimeWhileStanding;
	private float currentTimeOnTopOfPlatform;
	private GameManager gameManager;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		bumper = GetComponentInChildren<Bumper> ();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		bumpCooldownTimer = 0;
		hitstunTimer = 0;
		comboCounter = 0;
		currentTimeOnTopOfPlatform = 0;
		bumperCooldownObj = GetComponentsInChildren<Transform> () [2];
	}
	
	// Update is called once per frame
	void Update () {
		previousVelocity = rb.velocity;
	}

	void FixedUpdate(){
		CheckIfGrounded ();
		if (hitstunTimer > 0) {
			hitstunTimer -= Time.deltaTime;
			UpdateCooldownTimerObj ();
		} else {
			ProcessInput ();
		}
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

	public void GetHit(Vector2 knockback){
		if (hitstunTimer > 0) {
			comboCounter += 1;
		}
		GetBumped (knockback, true, true);
		hitstunTimer = hitstunDuration * (1 + (comboScaling * comboCounter));
		bumpCooldownTimer = 0;
		UpdateCooldownTimerObj ();
	}

	public void GetBumped(Vector2 bumpVector, bool vertical, bool horizontal){
		Vector2 newVelocity = Vector2.zero;
		if (vertical) {
			newVelocity = new Vector2 (newVelocity.x, bumpVector.y);
		} 
		if (horizontal) {
			newVelocity = new Vector2 (bumpVector.x, newVelocity.y);
		}
		rb.velocity = newVelocity;
	}

	bool CheckIfGrounded(){
		bool grounded;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.down, groundDetectionDistance, groundLayer);
		Debug.DrawRay (transform.position, Vector2.down * groundDetectionDistance);
		if (hit) {
			grounded = true;
			currentTimeOnTopOfPlatform += Time.deltaTime;
			if (currentTimeOnTopOfPlatform >= platformLifetimeWhileStanding) {
				Block block = hit.collider.gameObject.GetComponent<Block> ();
				block.DestroyThis ();
			}
		} else {
			grounded = false;
			currentTimeOnTopOfPlatform = 0;
		}
		return grounded;
	}

	void OnCollisionEnter2D(Collision2D collision){
		GameObject obj = collision.collider.gameObject;
		Vector3 launchVector;
		if (obj.tag == "Surface") {
			launchVector = previousVelocity * blockBounceFactor;
			launchVector = new Vector2 (launchVector.x, -launchVector.y);
			if (hitstunTimer > 0) {
				GetHit (launchVector);
				obj.transform.parent.gameObject.GetComponent<Block> ().DestroyThis ();
			} else {
				GetBumped (launchVector, true, true);
			}
		} else if (obj.tag == "Player") {
			launchVector = previousVelocity * -playerBounceFactor;
			GetBumped (launchVector, true, true);
		} else if (obj.tag == "DeathZone") {
			Destroy (gameObject);
			if (!gameManager.gameOver) {
				gameManager.gameOver = true;
				gameManager.GameOver (playerNum);
			}
		}
	}

	void UpdateCooldownTimerObj(){
		if (hitstunTimer > 0) {
			bumperCooldownObj.localScale = Vector3.one * Mathf.Lerp (0, 1, 1 - (hitstunTimer / hitstunDuration));
			bumperCooldownObj.GetComponent<SpriteRenderer> ().color = Color.grey;
		} else {
			bumperCooldownObj.localScale = Vector3.one * Mathf.Lerp (0, 1, 1 - (bumpCooldownTimer / bumpCooldown));
			bumperCooldownObj.GetComponent<SpriteRenderer> ().color = defaultColor;
		}
	}
}
