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
    public float bounceMinSpd;
    public float underBumpCut;
    public float bumpBounceScale;
    public float bumpPlayerScale;
    private Rigidbody2D rb;
    private Vector2 previousVelocity;
    public bool bump;
    public int bounced = 0;
    public float wallKickCut;
    public float wallKickMinSpeed;
    public float sideCollisionOffset;

    public bool stun;
    public float stunTimeLength;
    public float stunTimeUntil;

    public float bounceTime = 0;

    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        GetComponent<SpriteRenderer>().color = color;
        UnlockAllInput();
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        previousVelocity = rb.velocity;
        if (actionable)
        {
            Move();
        }

        // temporary death function
        if (transform.position.y < -150)
        {
            Die();
        }
    }

    void Move()
    {
        //Input Handling with the joysticks of the controllers
        float input = Input.GetAxis("Horizontal_P" + playerNum);
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
            float velocityX = previousVelocity.x;
            float velocityY = previousVelocity.y;

            if (bump)
            {
                Debug.Log("bounce");
                velocityY = previousVelocity.y * bumpBounceScale - bounceMinSpd;
                obj.GetComponent<Block>().DestroyThis();
                Services.EventManager.Fire(new BumpHit(this));
            }
            else
            {
                velocityY = previousVelocity.y * bounceScale;
            }
            //Check if hitting from below
            if (transform.position.y < obj.GetComponent<SpriteRenderer>().bounds.min.y)
            {
                velocityY = velocityY * (1.0f - underBumpCut);
            }
            //Check if hitting left side of the block
            //if (GetComponent<SpriteRenderer>().bounds.max.x - sideCollisionOffset < obj.GetComponent<SpriteRenderer>().bounds.min.x)
			if (transform.position.x - sideCollisionOffset < obj.GetComponent<SpriteRenderer>().bounds.min.x)
            {
                velocityX = -previousVelocity.x * (1.0f - wallKickCut) - wallKickMinSpeed;
                //bounceVector = new Vector2 (-previousVelocity.x, bounceVector.y);
                Debug.Log("hit on the left side");
            }
            //Check if hitting right side of the block
            //else if (GetComponent<SpriteRenderer>().bounds.min.x + sideCollisionOffset > obj.GetComponent<SpriteRenderer>().bounds.max.x)
			else if (transform.position.x + sideCollisionOffset > obj.GetComponent<SpriteRenderer>().bounds.max.x)
            {
                velocityX = -previousVelocity.x * (1.0f - wallKickCut) + wallKickMinSpeed;
                //bounceVector = new Vector2 (-previousVelocity.x, bounceVector.y);
                Debug.Log("hit on the right side");
            }
            rb.velocity = new Vector2(velocityX, -velocityY);
        }

        if (obj.tag == "Player")
        {
            Player enemy = collision.gameObject.GetComponent<Player>();
            bool sameDirection = false;
            float velocityX = previousVelocity.x;
            float velocityY = previousVelocity.y;
            if (enemy.bump)
            {
                if (!bump)
                {
                    velocityX = enemy.previousVelocity.x;
                    velocityY = enemy.previousVelocity.y;
                    GetStunned(stunTimeLength);
                }
            }
            else
            {
                if (bump)
                {
                    if (Mathf.Sign(previousVelocity.x) != Mathf.Sign(enemy.previousVelocity.x))
                    {
                        velocityX = previousVelocity.x + enemy.previousVelocity.x * .1f;

                    }
                    else
                    {
                        velocityX -= previousVelocity.x - enemy.previousVelocity.x;
                    }
                    if (Mathf.Sign(previousVelocity.y) != Mathf.Sign(enemy.previousVelocity.y))
                    {
                        velocityY = velocityY + enemy.previousVelocity.y;
                    }
                    else
                    {
                        velocityY = previousVelocity.y - enemy.previousVelocity.y * .1f;
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
        BumpTask bumpCooldownTask = new BumpTask(bumpCooldown, this);
        Services.TaskManager.AddTask(bumpCooldownTask);
    }

    public void GetStunned(float stunDuration)
    {
        StunTask hitstunTask = new StunTask(stunDuration, this);
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
