using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fire : MonoBehaviour {

	private Player player;
    public float fireRadiusRatio;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    // Use this for initialization
    void Start () {
		changeColor (player.fireColor);
        changeGlow(player.fireGlowIntensity, player.fireGlowRange);
    }
	
	// Update is called once per frame
	void Update () {
        //this.gameObject.GetComponents<Transform>(). = Mathf.Atan2 (player.rb.velocity.x, player.rb.velocity.y) * 180 / Mathf.PI;
        //transform.rotation
    }

    public void updateSize()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.radius = fireRadiusRatio * player.transform.localScale.x;
    }

	public void changeColor(Gradient newColor) {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;

		col.color = newColor;
	}

    public void changeGlow(float intensity, float range)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var lights = ps.lights;
        lights.intensityMultiplier = intensity;
        lights.rangeMultiplier = range;
    }

	public void controlFlow(bool on) {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (on) {
			ps.Play();
		} else {
			ps.Stop();
		}
	}
}
