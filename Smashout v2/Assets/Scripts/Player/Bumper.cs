using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    private SpriteRenderer sprite;
    private CircleCollider2D collide;
    private Player player;
    public float playerBumpPower;
    public float kickback;
    public Color activeColor;
    public Color availableColor;

	// Use this for initialization
	void Start () {
        player = GetComponentInParent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<CircleCollider2D>();
        collide.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
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
            player.CollideWithSurface(obj, true);
        }
        if(obj.tag == "Player")
        {
            Player enemy = collision.gameObject.GetComponent<Player>();
            player.RefreshBumpPrivilege();
            Vector3 launchVector = (enemy.transform.position - player.transform.position).normalized * playerBumpPower;
            Vector2 kickbackVector = -launchVector * kickback;
            enemy.GetHit(launchVector);
            player.rb.velocity = kickbackVector;
        }
    }
}
