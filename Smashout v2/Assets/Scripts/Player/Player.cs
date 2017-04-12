using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Color color;
    public Gradient dashingTrailColor;
    public Gradient trailColor;
	public Gradient fireColor;
    public int playerNum;
    public float bumpActiveTime;
    private bool actionable;

    private Bumper bumper;
    public float moveSpeed;
    public float dashDriftSpeedFactor;
    public float bounceScale;
    public float bumpMinSpd;
    public float underBumpCut;
    public float dashSpeed;

	public float bumpBounceScale;
	public float bumpPlayerScale;
    public Rigidbody2D rb;
    private GameObject trailObj;
    public Vector2 previousVelocity;
    public bool bumpAvailable;
    public float defaultGravity;
    public bool dashing;

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

	public GameObject fireObj;
	public Fire fire;

    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bumper = GetComponentInChildren<Bumper>();
        trailObj = GetComponentInChildren<TrailRenderer>().gameObject;
		fireObj = GetComponentInChildren<ParticleSystem> ().gameObject;
		fireObj.SetActive (true);
		fire = GetComponentInChildren<Fire> ();
    }

    void Start()
    {
        currentTimeOnTopOfPlatform = 0f;
        GetComponent<SpriteRenderer>().color = color;
        trailObj.GetComponent<TrailRenderer>().colorGradient = trailColor;
        bumpAvailable = true;
        dashing = false;
        defaultGravity = rb.gravityScale;
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
            //Die();
        }
    }

    void Move()
    {
        //Input Handling with the joysticks of the controllers
        float input = Input.GetAxis("Horizontal_P" + playerNum);
        if (Mathf.Abs(input) > 0.1f)
        {
            Vector2 moveForce = new Vector2(input * moveSpeed, 0);
            if (dashing)
            {
                moveForce *= dashDriftSpeedFactor;
            }
            rb.AddForce(moveForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        if (obj.tag == "Surface")
        {
            if (obj.GetComponent<Block>().GetType() != typeof(DeathBlock))
            {
                CollideWithSurface(obj, stun);
            }
        }
        else if (obj.tag == "Player")
        {
            if (!stun)
            {
                RefreshBumpPrivilege();
            }
        }
    }

    void OnButtonPressed(ButtonPressed e)
    {
        string button = e.button;
        if (e.playerNum == playerNum)
        {
            if (button == "A" && bumpAvailable)
            {
                Bump();
            }
        }
    }

    void Bump()
    {
        bumpAvailable = false;
        Vector2 input = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum));
        if (input.magnitude > 0.1f)
        {
            Vector2 dashVector = input.normalized * dashSpeed;
            rb.velocity = dashVector;
        }
        BumpTask bumpTask = new BumpTask(this, bumpActiveTime);
        Services.TaskManager.AddTask(bumpTask);
    }

    public void SetTrailColor(bool dashing)
    {
        Gradient trailGradient;
        if (dashing)
        {
            trailGradient = dashingTrailColor;
        }
        else
        {
            trailGradient = trailColor;
        }
        trailObj.GetComponent<TrailRenderer>().colorGradient = trailGradient;
    }

	public void SetFireColor(bool dashing)
	{
		Gradient fireGradient;
		if (dashing)
		{
			fireGradient = dashingTrailColor;
		}
		else
		{
			fireGradient = fireColor;
		}
		fire.changeColor (fireGradient);
	}

    public void SetTrailActiveStatus(bool status)
    {
        trailObj.SetActive(status);
    }

	public void SetFireActiveStatus(bool status)
	{
		//fireObj.SetActive(status);
		fire.controlFlow(status);
	}

    public void RefreshBumpPrivilege()
    {
        bumpAvailable = true;
        SetTrailActiveStatus(true);
		SetFireActiveStatus (true);
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

        RefreshBumpPrivilege();

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

    public void Die()
    {
        gameObject.SetActive(false);
		fireObj.SetActive(false);
        Services.EventManager.Fire(new GameOver(playerNum));
    }

    void OnGameOver(GameOver e)
    {
        LockAllInput();
    }
}
