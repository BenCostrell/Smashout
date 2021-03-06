﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    //public List<Sprite> damageSprites;
    //public List<Color> damageColors;
    public List<GameObject> damageStates;
    public int damage;
    public float shiftDuration;
    public float shiftFactor;
    public AudioClip explosionSound;
    public AudioClip bounceSound;
    private AudioSource audioSrc;
    public float deathTime;
    public float appearanceTime;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    void Start () {
        Init();
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) s.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") OnCollideWithPlayer(collision);
    }

    protected virtual void OnCollideWithPlayer(Collision2D collision)
    {
        Vector3 prevVel = collision.gameObject.GetComponent<Player>().previousVelocity;
        DamageThis();
        BlockShift shift = new BlockShift(this,  prevVel * shiftFactor, shiftDuration);
        Services.TaskManager.AddTask(shift);
        audioSrc.clip = bounceSound;
        audioSrc.volume = prevVel.magnitude / 200f;
        audioSrc.Play();       
    }
    
    public virtual void OnBumpedByPlayer(Player player)
    {
        DestroyThis(true);
    }

    protected virtual void Init()
    {
    }

    public virtual void DamageThis()
    {
        if (++damage < damageStates.Count)
        {
            Services.TaskManager.AddTask(new BlockDamage(this, shiftDuration));
        }
        else DestroyThis(true);
    }

    public virtual void DestroyThis(bool playSound)
    {
        Services.BlockManager.DestroyBlock(this, true, playSound);
    }

    public virtual void StartDestructionAnimation(bool playSound)
    {
        BlockFadeOut fadeOut = new BlockFadeOut(gameObject, deathTime);
        Services.TaskManager.AddTask(fadeOut);
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) ps.Play();
        if (playSound)
        {
            audioSrc.clip = explosionSound;
            audioSrc.volume = 1;
            audioSrc.Play();
        }
    }

    public virtual void StartAppearanceAnimation()
    {
        BlockAppear appear = new BlockAppear(gameObject, appearanceTime);
        Services.TaskManager.AddTask(appear);
    }
}
