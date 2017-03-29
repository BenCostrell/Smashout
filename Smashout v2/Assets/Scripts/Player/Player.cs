using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Color color;
    public int playerNum;
    public float bumpCooldown;
	private float bumpTime;
	public float hurtScale;
    private bool actionable;

    public float moveSpeed;
    public float xDrag;
    public float maxVelocity;
    public float bounceScale;
	public float bumpBounceScale;
	public float bumpPlayerScale;
    private Rigidbody2D rb;
    private Vector2 previousVelocity;
	public bool bump;
    public int bounced = 0;

	public bool stun;
	public float stunTimeLenght;
	public float stunTimeUntil;

	public float bounceTime = 0;

    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        GetComponent<SpriteRenderer> ().color = color;
        UnlockAllInput();
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        previousVelocity = rb.velocity;
        if (actionable && !stun)
        {
            Move();
			if (bump) {
				if (Time.time >= bumpTime) {
					bump = false;
					GetComponent<SpriteRenderer> ().color = color;
				}
				if (Time.time >= stunTimeUntil) {
					stun = false;
					GetComponent<SpriteRenderer> ().color = color;
				}
			}
        }
    }

    void Move()
    {
        //Input Handling with the joysticks of the controllers
        float input = Input.GetAxis("Horizontal_P" + playerNum);
        Debug.Log(input + " from player " + playerNum);
        Vector2 moveForce = new Vector2(input * moveSpeed, 0);
        rb.AddForce(moveForce);
        /*Vector2 addDrag = new Vector2(-input*xDrag, 0);
        rb.AddForce(addDrag);
        if(Mathf.Abs(rb.velocity.x) > maxVelocity)
        {
            rb.velocity = new Vector2((rb.velocity.x / Mathf.Abs(rb.velocity.x)) * maxVelocity, rb.velocity.y);
        }*/

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        Vector2 bounceVector;
        if (obj.tag == "Surface")
        {
			if (bump) {
				bounceVector = previousVelocity * bumpBounceScale;
				rb.velocity = new Vector2 (previousVelocity.x, -bounceVector.y);
			} else {
				//rb.AddForce(new Vector2(0, Mathf.Abs(rb.velocity.y * 200)));
				bounceVector = previousVelocity * bounceScale;

				//for more sudden jerk bounce, do bounceVector.x
				rb.velocity = new Vector2 (previousVelocity.x, -bounceVector.y);
				//rb.velocity = new Vector2(bounceVector.x / 4, -bounceVector.y);
       
				/*
            float velocityX = previousVelocity.x;
            float velocityY = previousVelocity.y;
			*/

				/*
            if(transform.localPosition.y <= obj.transform.localPosition.y)
            {
                velocityY = Mathf.Abs(bounceVector.y);
                //rb.velocity = new Vector2(previousVelocity.x, Mathf.Abs(bounceVector.y));
            }
            else if(transform.localPosition.y > obj.transform.localPosition.y)
            {
                velocityY = -Mathf.Abs(previousVelocity.y);
                //rb.velocity = new Vector2(previousVelocity.x, -Mathf.Abs(bounceVector.y));
            }
            rb.velocity = new Vector2(velocityX, velocityY);
            */
			}
        }
		if (obj.tag == "Player")
		{
			Player enemy = collision.gameObject.GetComponent<Player>();
			bool sameDirection = false;
			float velocityX = previousVelocity.x;
			float velocityY = previousVelocity.y;
			if (enemy.bump) {
				if (!bump) {
					velocityX = enemy.previousVelocity.x;
					velocityY = enemy.previousVelocity.y;
					/*
					if (Mathf.Sign (previousVelocity.x) != Mathf.Sign (enemy.previousVelocity.x)) {
						velocityX = enemy.previousVelocity.x;

					} else {
						velocityX = enemy.previousVelocity.x;
					}
					if (Mathf.Sign (previousVelocity.y) != Mathf.Sign (enemy.previousVelocity.y)) {
						velocityY = velocityY + enemy.previousVelocity.y;
					} else {
						velocityY -= previousVelocity.y - enemy.previousVelocity.y;
					}
					*/
					stun = true;
					GetComponent<SpriteRenderer> ().color = Color.grey;
				}
			} else {
				if (bump) {
					if (Mathf.Sign (previousVelocity.x) != Mathf.Sign (enemy.previousVelocity.x)) {
						velocityX = previousVelocity.x + enemy.previousVelocity.x*.1f;

					} else {
						velocityX -= previousVelocity.x - enemy.previousVelocity.x;
					}
					if (Mathf.Sign (previousVelocity.y) != Mathf.Sign (enemy.previousVelocity.y)) {
						velocityY = velocityY + enemy.previousVelocity.y;
					} else {
						velocityY = previousVelocity.y - enemy.previousVelocity.y*.1f;
					}
				}
			}
			rb.velocity = new Vector2(velocityX, velocityY);
		}
    }

    void OnButtonPressed(ButtonPressed e)
    {
        string button = e.button;
        if (e.playerNum == playerNum)
        {
            if (button == "A")
            {
                Bump();
            }
        }
    }

    void Bump()
    {
        LockOutButtonInput bumpCooldownTask = new LockOutButtonInput(bumpCooldown, this);
        Services.TaskManager.AddTask(bumpCooldownTask);
		GetComponent<SpriteRenderer>().color = Color.yellow;
		bump = true;
		bumpTime = Time.time + bumpCooldown;
    }

    public void GetStunned(float stunDuration)
    {
        LockOutAllInput hitstunTask = new LockOutAllInput(stunDuration, this);
        Services.TaskManager.AddTask(hitstunTask);
    }

    public void LockButtonInput()
    {
        Services.EventManager.Unregister<ButtonPressed>(OnButtonPressed);
    }

    public void LockAllInput()
    {
        LockButtonInput();
        actionable = false;
    }

    public void UnlockButtonInput()
    {
        Services.EventManager.Register<ButtonPressed>(OnButtonPressed);
    }

    public void UnlockAllInput()
    {
        UnlockButtonInput();
        actionable = true;
    }

    void Die()
    {
        LockAllInput();
        Services.EventManager.Fire(new GameOver(playerNum));
    }

    void OnGameOver(GameOver e)
    {
    }
}
