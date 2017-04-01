using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Color color;
    public int playerNum;
    public float bumpCooldown;
    public float bumpActiveTime;
    public float hurtScale;
    private bool actionable;

    private Bumper bumper;
    public float moveSpeed;
    public float xDrag;
    public float maxVelocity;
    public float bounceScale;
    public float bumpMinSpd;
    public float underBumpCut;
    public float dashSpeed;

	public float bumpBounceScale;
	public float bumpPlayerScale;
    public Rigidbody2D rb;
    private TrailRenderer tr;
    public Vector2 previousVelocity;
	public bool bump;

    public int bounced = 0;
    public float wallKickCut;
    public float wallKickMinSpeed;
    public float sideCollisionOffset;

    public bool stun;
    public float stunTimeLength;
    public float stunTimeUntil;

    public float groundDetectionDistance;
    public LayerMask groundLayer;
    private float currentTimeOnTopOfPlatform;
    public float platformLifetimeWhileStanding;

	public float bounceTime = 0;

    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bumper = GetComponentInChildren<Bumper>();
        tr = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        currentTimeOnTopOfPlatform = 0f;
        GetComponent<SpriteRenderer>().color = color;
        tr.enabled = false;
        UnlockAllInput();
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
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
        if (Mathf.Abs(input) > 0.1f)
        {
            Vector2 moveForce = new Vector2(input * moveSpeed, 0);
            rb.AddForce(moveForce);
        }
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
        if (obj.tag == "Surface")
        {
            CollideWithSurface(obj, stun);
        }
		if (obj.tag == "Player")
		{

			/*float velocityX = previousVelocity.x;
			float velocityY = previousVelocity.y;
			
			rb.velocity = new Vector2(-velocityX/4, -velocityY/4);*/
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
        /*Vector2 input = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum));
        if (input.magnitude > 0.1f)
        {
            Vector2 dashVector = input.normalized * dashSpeed;
            rb.velocity = dashVector;
        }*/
        BumpTask bumpCooldownTask = new BumpTask(bumpCooldown, this, bumpActiveTime);
        Services.TaskManager.AddTask(bumpCooldownTask);
    }

    public void SetTrailStatus(bool active)
    {
        tr.enabled = active;
    }

    public void GetStunned(float stunDuration)
    {
        StunTask hitstunTask = new StunTask(stunDuration, this);
        Services.TaskManager.AddTask(hitstunTask);
    }

    public void GetHit(Vector3 hitVector)
    {
        GetStunned(stunTimeLength);
        rb.velocity = hitVector;
    }

    public void CollideWithSurface(GameObject surface, bool bump)
    {
        float velocityX = previousVelocity.x;
        float velocityY = -previousVelocity.y * bounceScale;
        float scaling = 1f;
        if (bump)
        {
            scaling = bumpBounceScale;
            if (transform.position.y > surface.GetComponent<SpriteRenderer>().bounds.min.y)
            {
                velocityY = Mathf.Abs(velocityY) + bumpMinSpd;
            }
            surface.GetComponent<Block>().DestroyThis();
            Services.EventManager.Fire(new BumpHit(this));
        }

        //Check if hitting from below
        if (transform.position.y < surface.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            velocityY = velocityY * (1.0f - underBumpCut);
        }
        //Check if hitting left side of the block
        if (transform.position.x - sideCollisionOffset < surface.GetComponent<SpriteRenderer>().bounds.min.x)
        {
            velocityX = -Mathf.Abs(previousVelocity.x * (1.0f - wallKickCut)) - wallKickMinSpeed;
        }
        //Check if hitting right side of the block
        else if (transform.position.x + sideCollisionOffset > surface.GetComponent<SpriteRenderer>().bounds.max.x)
        {
            velocityX = Mathf.Abs(previousVelocity.x * (1.0f - wallKickCut)) + wallKickMinSpeed;
        }

        //---------------------------//
        rb.velocity = new Vector2(velocityX, velocityY) * scaling;
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

    void CheckIfGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundDetectionDistance, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundDetectionDistance);
        if (hit)
        {
            currentTimeOnTopOfPlatform += Time.deltaTime;
            if (currentTimeOnTopOfPlatform >= platformLifetimeWhileStanding)
            {
                Block block = hit.collider.gameObject.GetComponent<Block>();
                block.DestroyThis();
            }
        }
        else {
            currentTimeOnTopOfPlatform = 0;
        }
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
