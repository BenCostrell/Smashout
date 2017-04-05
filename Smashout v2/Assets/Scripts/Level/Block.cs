using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public List<Sprite> damageSprites;
    public List<Color> damageColors;
    public int damage;
    private SpriteRenderer spr;

    void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sprite = damageSprites[damage];
        spr.color = damageColors[damage];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") DamageThis();
    }

    public void DamageThis()
    {
        if (++damage < damageSprites.Count)
        {
            spr.sprite = damageSprites[damage];
            spr.color = damageColors[damage];
        }
        else DestroyThis();
    }

    public void DestroyThis()
    {
        Services.BlockManager.DestroyBlock(this, true);
    }

    public void StartDestructionAnimation()
    {
        BlockFadeOut fadeOut = new BlockFadeOut(gameObject, Services.BlockManager.blockDeathTime);
        Services.TaskManager.AddTask(fadeOut);
    }

    public void StartAppearanceAnimation()
    {
        BlockAppear appear = new BlockAppear(gameObject, Services.BlockManager.blockAppearanceTime);
        Services.TaskManager.AddTask(appear);
    }
}
