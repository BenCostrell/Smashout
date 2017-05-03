using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderBlock : Block {
	void update() {}

    public override void DamageThis()
    {
        damage = 0;
    }

    public override void DestroyThis(bool playSound)
	{
	}
}
