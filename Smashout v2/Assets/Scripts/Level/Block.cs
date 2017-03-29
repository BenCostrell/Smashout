using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    int health = 1;

    void Start () {

	}

	void Update () {
        if(health < 1)
        {
            StartDestructionAnimation();
        }
	}

    public void DestroyThis()
    {
        Services.BlockManager.DestroyBlock(this);
    }

    public void StartDestructionAnimation()
    {
        Debug.Log("No block destroy animation yet!");
        //iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack,
        //    "time", blockManager.blockDeathTime));
    }

    public void StartAppearanceAnimation()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        Debug.Log("No block appear animation yet!");
        //iTween.ScaleFrom(gameObject, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeOutBack,
        //    "time", blockManager.blockAppearanceTime));
    }
}
