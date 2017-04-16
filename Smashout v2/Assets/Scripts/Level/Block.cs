using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public List<Sprite> damageSprites;
    public List<Color> damageColors;
    public int damage;
    private SpriteRenderer spr;
    public float shiftDuration;
    public float shiftFactor;

    void Start () {
        Init();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") OnCollideWithPlayer(collision);
    }

    protected virtual void OnCollideWithPlayer(Collision2D collision)
    {
        DamageThis();
        BlockShift shift = new BlockShift(this, collision.gameObject.GetComponent<Player>().previousVelocity * shiftFactor, shiftDuration);
        Services.TaskManager.AddTask(shift);          
    }
    
    public virtual void OnBumpedByPlayer(Player player)
    {
        DestroyThis(true);
    }

    protected virtual void Init()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sprite = damageSprites[damage];
        spr.color = damageColors[damage];
    }

    public void DamageThis()
    {
        if (++damage < damageSprites.Count)
        {
            spr.sprite = damageSprites[damage];
            spr.color = damageColors[damage];
        }
        else DestroyThis(true);
    }

    public void DestroyThis(bool playSound)
    {
        Services.BlockManager.DestroyBlock(this, true, playSound);
    }

    public virtual void StartDestructionAnimation(bool playSound)
    {
        BlockFadeOut fadeOut = new BlockFadeOut(gameObject, Services.BlockManager.blockDeathTime);
        Services.TaskManager.AddTask(fadeOut);
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) ps.Play();
        if (playSound)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public virtual void StartAppearanceAnimation()
    {
        BlockAppear appear = new BlockAppear(gameObject, Services.BlockManager.blockAppearanceTime);
        Services.TaskManager.AddTask(appear);
    }
}
