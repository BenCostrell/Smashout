using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    private SpriteRenderer sprite;
    private CircleCollider2D collide;
    private Player player;
    public bool active;
	// Use this for initialization
	void Start () {
        player = GetComponentInParent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<CircleCollider2D>();
        collide.enabled = false;
        sprite.enabled = false;
        active = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void setActiveStatus(bool status)
    {
        if (status)
        {
            sprite.enabled = true;
            collide.enabled = true;
            active = true;
        }
        else
        {
            sprite.enabled = false;
            collide.enabled = false;
            active = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        GameObject obj = collision.gameObject;
        Vector2 bounceVector;
        if (obj.tag == "Surface")
        {
            if (active)
            {
                Debug.Log("bounce");
                bounceVector = player.previousVelocity * player.bumpBounceScale - Vector2.up * player.bounceMinSpd;
                if (transform.position.y < obj.GetComponent<SpriteRenderer>().bounds.min.y)
                {
                    bounceVector = new Vector2(bounceVector.x, bounceVector.y * (1.0f - player.underBumpCut));
                }
                player.rb.velocity = new Vector2(player.previousVelocity.x, -bounceVector.y);
                obj.GetComponent<Block>().DestroyThis();
                Services.EventManager.Fire(new BumpHit(player));
            }
        }
        if(obj.tag == "Player")
        {
            Player enemy = collision.gameObject.GetComponent<Player>();
            float velocityX = enemy.previousVelocity.x;
            float velocityY = enemy.previousVelocity.y;
            if (active)
            {
                enemy.GetStunned(enemy.stunTimeLength);
                enemy.rb.velocity = new Vector2(velocityX, velocityY);
            }
        }
        if(obj.tag == "Bumper")
        {
            Bumper bump = collision.gameObject.GetComponent<Bumper>();
            Player enemy = collision.gameObject.GetComponentInParent<Player>();
            float velocityX = enemy.previousVelocity.x;
            float velocityY = enemy.previousVelocity.y;
            if (active)
            {
                if (Mathf.Sign(player.previousVelocity.x) != Mathf.Sign(enemy.previousVelocity.x))
                {
                    velocityX = player.previousVelocity.x + enemy.previousVelocity.x * .1f;

                }
                else
                {
                    velocityX -= player.previousVelocity.x - enemy.previousVelocity.x;
                }
                if (Mathf.Sign(player.previousVelocity.y) != Mathf.Sign(enemy.previousVelocity.y))
                {
                    velocityY = velocityY + enemy.previousVelocity.y;
                }
                else
                {
                    velocityY = player.previousVelocity.y - enemy.previousVelocity.y * .1f;
                }
            }
        }
    }
}
