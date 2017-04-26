using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Color color;
    public Gradient dashingTrailColor;
    [HideInInspector]
    public Gradient trailColor;
    [HideInInspector]
	public Gradient fireColor;
    public float fireGlowIntensity;
    public float fireGlowRange;
    public float dashGlowIntensity;
    public float dashGlowRange;
    [HideInInspector]
    public int playerNum;
    public float bumpActiveTime;
    private bool actionable;

    private Bumper bumper;
    public float accel;
    public float maxSpeed;
    public float slowdownRate;
    public float dashDriftSpeedFactor;
    public float bounceScale;
    public float bumpMinSpd;
    public float underBumpCut;
    public float dashSpeed;
    public float squashAndStretchFactor;
    public float hitSlowIntensity;
    public float hitSlowDuration;

	public float bumpBounceScale;
	public float bumpPlayerScale;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public AudioSource audioSrc;
    private GameObject trailObj;
    [HideInInspector]
    public Vector2 previousVelocity;
    [HideInInspector]
    public bool bumpAvailable;
    [HideInInspector]
    public float defaultGravity;
    [HideInInspector]
    public bool dashing;

    public float wallKickCut;
    public float wallKickMinSpeed;
    public float sideCollisionOffset;

    [HideInInspector]
    public bool stun;
    public float stunTimeLength;
    
    public float groundDetectionDistance;
    public LayerMask groundLayer;
    private float currentTimeOnTopOfPlatform;
    public float platformLifetimeWhileStanding;

    [HideInInspector]
	public GameObject fireObj;
    [HideInInspector]
	public Fire fire;
    private ParticleSystem sparks;
    public ParticleSystem rings;
    private Camera offscreenCamera;
    public int basePower;
    public int maxPower;
    public float baseRadius;
    public float radiusPerPowerUnit;
    public float powerUpAnimationLength;
    private bool powerInitialized;
    public AudioClip powerUpAudio;
    public AudioClip bumpPlayerHitAudio;
    public AudioClip bounce;

    [HideInInspector]
    public int power
    {
        get { return _power; }
        set
        {
            int newPower = Mathf.Clamp(value, 0, maxPower);
            if (newPower > _power && powerInitialized)
            {
                PowerUpAnimation powerUpAnim = new PowerUpAnimation(this, powerUpAnimationLength);
                Services.TaskManager.AddTask(powerUpAnim);
                audioSrc.clip = powerUpAudio;
                audioSrc.Play();
            }
            _power = newPower;
            float rad = baseRadius + _power * radiusPerPowerUnit;
            transform.localScale = new Vector3(rad, rad, transform.localScale.z);
            fire.updateSize();
        }
    }
    private int _power;

    // Use this for initialization

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSrc = GetComponent<AudioSource>();
        bumper = GetComponentInChildren<Bumper>();
        trailObj = GetComponentInChildren<TrailRenderer>().gameObject;
		fireObj = GetComponentsInChildren<ParticleSystem> ()[0].gameObject;
		fireObj.SetActive (true);
		fire = GetComponentInChildren<Fire> ();
        sparks = GetComponentsInChildren<ParticleSystem>()[1];
        rings = GetComponentsInChildren<ParticleSystem>()[2];
        offscreenCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        power = basePower;
        powerInitialized = true;
        currentTimeOnTopOfPlatform = 0f;
        GetComponent<SpriteRenderer>().color = color;
        trailObj.GetComponent<TrailRenderer>().colorGradient = trailColor;
        ParticleSystem.MainModule settings = rings.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(color);
        bumpAvailable = true;
        dashing = false;
        defaultGravity = rb.gravityScale;
        UnlockAllInput();
        SetTrailActiveStatus(false);
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        previousVelocity = rb.velocity;
        Turn();
        if (actionable)
        {
            Move();
        }
        SquashAndStretch();
    }

    void Turn()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        offscreenCamera.transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    void SquashAndStretch()
    {
        transform.localScale = new Vector3(
            (baseRadius + power * radiusPerPowerUnit) * (1 + (rb.velocity.magnitude * squashAndStretchFactor)),
            (baseRadius + power * radiusPerPowerUnit) * (1 - (rb.velocity.magnitude * squashAndStretchFactor)), 
            transform.localScale.z);
    }

    void Move()
    {
        //Input Handling with the joysticks of the controllers
        float[] input = { Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum) };
        if (Mathf.Sqrt(input[0]* input[0] + input[1]*input[1]) > 0.1f)
        {
            Vector2 moveForce = new Vector2(input[0] * accel, 0);
            if (dashing)
            {
                moveForce += new Vector2(0, input[1] * accel);
                Vector2 ortho = rb.velocity;
                ortho = new Vector2(-ortho.y, ortho.x);
                if (Vector2.Dot(ortho, moveForce) < 0) ortho *= -1.0f;
                moveForce = Vector3.Project(moveForce, ortho);
                moveForce *= dashDriftSpeedFactor;
            }
            rb.AddForce(moveForce);
        }
        if (!dashing && !stun && rb.velocity.magnitude > maxSpeed)
        {
            Vector2 difference = rb.velocity - rb.velocity.normalized * maxSpeed;
            if(rb.velocity.y <= 0)
            {
                rb.velocity -= new Vector2(difference.x * slowdownRate, 0);
            }
            else rb.velocity -= difference * slowdownRate;
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
            if (!stun && !obj.GetComponent<Player>().dashing)
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
            if (button == "A")
            {
                if (sparks != null)
                {
                    sparks.Play();
                }
                if (bumpAvailable)
                {
                    Bump();
                }
            }
        }
    }

    public void Bump()
    {
        bumpAvailable = false;
        Vector2 input = new Vector2(Input.GetAxis("Horizontal_P" + playerNum), Input.GetAxis("Vertical_P" + playerNum));
        if (input.magnitude > 0.1f)
        {
            Vector2 dashVector = input.normalized * dashSpeed;
			//if (rb == null) rb = GetComponent<Rigidbody2D> ();
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

    public void SetFireGlow(bool dashing)
    {
        float intensity;
        float range;
        if (dashing)
        {
            intensity = dashGlowIntensity;
            range = dashGlowRange;
        }
        else
        {
            intensity = fireGlowIntensity;
            range = fireGlowRange;
        }
        fire.changeGlow(intensity, range);
    }

    public void SetTrailActiveStatus(bool status)
    {
        trailObj.SetActive(false);
    }

	public void SetFireActiveStatus(bool status)
	{
		fire.controlFlow(status);
	}

    public void PlayHitAudio()
    {
        audioSrc.clip = bumpPlayerHitAudio;
        audioSrc.Play();
    }

    void PlayHitShockwave(Player enemy)
    {
        Vector3 differenceVector = enemy.transform.position - transform.position;
        Vector3 collisionPoint = transform.position + differenceVector / 2;
        float collisionAngle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;

        ParticleSystem hitShockwave = 
            Instantiate(Services.PrefabDB.HitShockwave, collisionPoint, Quaternion.identity).GetComponent<ParticleSystem>();

        var psMain = hitShockwave.main;
        psMain.startRotation = collisionAngle;
        PlayParticleSystemUnscaledTime playHitShockwave = new PlayParticleSystemUnscaledTime(hitShockwave);
        Services.TaskManager.AddTask(playHitShockwave);
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

    public void InitiateBumpHit(Player enemy)
    {
        Services.EventManager.Fire(new BumpHit(this));
        PlayHitShockwave(enemy);
        PlayHitAudio();
        
        //ZoomInOnHit zoomIn = new ZoomInOnHit(hitSlowDuration, collisionPoint);
        //ReturnCameraToNormal returnCameraToNormal = new ReturnCameraToNormal(hitSlowDuration); 
        enemy.stun = true;

        ListenToQueueUpDash listenToQueueDash = new ListenToQueueUpDash(this, hitSlowDuration);
        SlowMoTask slowMo = new SlowMoTask(hitSlowIntensity, hitSlowDuration);
        WaitForTime waitToResolveHit = new WaitForTime(hitSlowDuration / 2);
        ResolveBumpHit resolveHit = new ResolveBumpHit(this, enemy);
        ActionTask refreshBump = new ActionTask(RefreshBumpPrivilege);
        waitToResolveHit.Then(resolveHit);
        slowMo.Then(refreshBump);
        //zoomIn.Then(returnCameraToNormal);
        //Services.TaskManager.AddTask(zoomIn);
        Services.TaskManager.AddTask(slowMo);
        Services.TaskManager.AddTask(waitToResolveHit);
        Services.TaskManager.AddTask(listenToQueueDash);
    }

    public void ResolveBumpHit(Player enemy)
    {
        Vector3 baseLaunchVector = (enemy.transform.position - transform.position).normalized
            * (bumper.playerBumpPower + (basePower * bumper.powerBumpRatio));
        Vector2 kickbackVector = -baseLaunchVector * bumper.kickback;
        Vector3 launchVector;

        if (!enemy.dashing)
        {
            launchVector = (enemy.transform.position - transform.position).normalized
            * (bumper.playerBumpPower + (power * bumper.powerBumpRatio));
        }
        else
        {
            launchVector = -kickbackVector;
        }

        if (transform.position.y < enemy.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            kickbackVector = new Vector3(kickbackVector.x, kickbackVector.y * (1 - underBumpCut));
        }

        OnBumperHittingOpponent(kickbackVector);
        enemy.OnHitByBumper(launchVector);
    }

    public void OnHitByBumper(Vector3 launchVector)
    {
        GetHit(launchVector);
    }

    public void OnBumperHittingOpponent(Vector3 kickbackVector)
    {
        //RefreshBumpPrivilege();
        rb.velocity = kickbackVector;
    }

    public void CollideWithSurface(GameObject surface, bool bump)
    {
        float velocityX = previousVelocity.x;
        float velocityY = previousVelocity.y * bounceScale;
        float scaling = 1f;
        if (bump)
        {
            scaling = bumpBounceScale;
//            if (transform.position.y > surface.GetComponent<SpriteRenderer>().bounds.min.y)
//            {
//				velocityY = (velocityY == 0 ? dashSpeed : Mathf.Abs(velocityY) + bumpMinSpd);
//            }
            surface.GetComponent<Block>().OnBumpedByPlayer(this);
            Services.EventManager.Fire(new BumpHit(this));
        }

        RefreshBumpPrivilege();

        //Check if hitting from below
		if (transform.position.y < surface.GetComponent<SpriteRenderer> ().bounds.min.y) {
			velocityY = -velocityY * (1.0f - underBumpCut);
		}
		else if (transform.position.y > surface.GetComponent<SpriteRenderer> ().bounds.max.y) {
			velocityY *= -1.0f;
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
                block.DestroyThis(true);
            }
        }
        else {
            currentTimeOnTopOfPlatform = 0;
        }
    }

    public void Die()
    { 
		Debug.Log ("Player Dies");
        gameObject.SetActive(false);
		fireObj.SetActive(false);
        LockAllInput();
		Debug.Log (playerNum);
        Services.EventManager.Fire(new GameOver(playerNum));
    }

    void OnGameOver(GameOver e)
    {
        LockAllInput();
    }

}
