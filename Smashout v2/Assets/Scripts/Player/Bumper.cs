using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    private SpriteRenderer sprite;
    private CircleCollider2D collide;
    private Player player;
    public float playerBumpPower;
    public float powerBumpRatio;
    public float kickback;
    public Color activeColor;
    public Color availableColor;
    private bool bumperHit = false;

	// Use this for initialization
	void Start () {
        player = GetComponentInParent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<CircleCollider2D>();
        collide.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        bumperHit = false;

		if (collide.enabled)
        {
            sprite.enabled = true;
            sprite.color = activeColor;
        }
        /* else if (player.bumpAvailable)
        {
            sprite.enabled = true;
            sprite.color = availableColor;
        }*/
        else
        {
            sprite.enabled = false;
        }
	}

    internal void setActiveStatus(bool status)
    {
        if (status)
        {
            collide.enabled = true;
        }
        else
        {
            collide.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        if (obj.tag == "Surface")
        {
            if (obj.GetComponent<Block>().GetType() != typeof(DeathBlock))
            {
                player.CollideWithSurface(obj, true);
            }
        }
        if(obj.tag == "Bumper")
        {
            Player enemy = obj.GetComponent<Bumper>().player;
            Services.EventManager.Fire(new BumpHit(player));
            if(player.dashing) player.RefreshBumpPrivilege();
            if(enemy.dashing) enemy.RefreshBumpPrivilege(); //not sure if this line is needed or not (I think not), but just in case
            Vector3 launchVector =
                (enemy.transform.position - player.transform.position).normalized * (playerBumpPower + (player.power * powerBumpRatio));
            Vector2 kickbackVector = -launchVector * kickback;
            enemy.GetHit(-kickbackVector);
            player.rb.velocity = kickbackVector;
            SlowMoTask slowMo = new SlowMoTask(player.hitSlowIntensity, player.hitSlowDuration);
            Services.TaskManager.AddTask(slowMo);
            player.audioSrc.clip = player.bumpPlayerHitAudio;
            player.audioSrc.Play();
            bumperHit = true;
        }

        if (obj.tag == "Player" && !bumperHit)
        {
            Player enemy = obj.GetComponent<Player>();
            Services.EventManager.Fire(new BumpHit(player));
            if(player.dashing || (!player.dashing && !enemy.dashing)) player.RefreshBumpPrivilege();
            Vector3 launchVector =
                (enemy.transform.position - player.transform.position).normalized * (playerBumpPower + (player.power * powerBumpRatio));
            Vector2 kickbackVector = -launchVector * kickback;
            enemy.GetHit(launchVector);
            player.rb.velocity = kickbackVector;
            SlowMoTask slowMo = new SlowMoTask(player.hitSlowIntensity, player.hitSlowDuration);
            Services.TaskManager.AddTask(slowMo);
            player.audioSrc.clip = player.bumpPlayerHitAudio;
            player.audioSrc.Play();
        }

        //if the tag = Surface, we have already handled this part
        if (obj.tag != "Surface")
        {
            Player enemy = obj.GetComponent<Player>();
            if (enemy == null) enemy = obj.GetComponentInParent<Player>();
            //if this player is dashing or neither the player or enemey are dashing 
            if (player.dashing || !enemy.dashing)
            {
                //cut the player's downward velocity by the underBumpCut if they are below the enemy
                if (transform.position.y < obj.GetComponent<SpriteRenderer>().bounds.min.y)
                {
                    player.rb.velocity = new Vector3(player.rb.velocity.x, player.rb.velocity.y * (1.0f - player.underBumpCut));
                }
                //otherwise, do the same for the enemy so long as both the player and enemy are dashing or neither are dashing
                else if(enemy.dashing || !player.dashing)
                {
                    enemy.rb.velocity = new Vector3(enemy.rb.velocity.x, enemy.rb.velocity.y * (1.0f - enemy.underBumpCut));
                }
            }
        }
    }
}
