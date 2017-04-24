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

	// Use this for initialization
	void Start () {
        player = GetComponentInParent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        collide = GetComponent<CircleCollider2D>();
        setActiveStatus(false);
	}
	
	// Update is called once per frame
	void Update () {
	}

    internal void setActiveStatus(bool status)
    {
        if (status)
        {
            collide.enabled = true;
            sprite.enabled = true;
            sprite.color = activeColor;
        }
        else
        {
            collide.enabled = false;
            sprite.enabled = false;
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
        if(obj.tag == "Bumper" || obj.tag == "Player")
        {
            Player enemy;
            if (obj.tag == "Bumper")
            {
                enemy = obj.GetComponent<Bumper>().player;
            }
            else
            {
                enemy = obj.GetComponent<Player>();
            }
            player.InitiateBumpHit(enemy);
        }
    }
}
