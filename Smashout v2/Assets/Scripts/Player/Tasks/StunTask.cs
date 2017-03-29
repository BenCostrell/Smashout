using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTask : LockOutAllInput {

    public StunTask(float dur, Player pl) : base(dur, pl) { }

    protected override void Init()
    {
        base.Init();
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = 0.4f * player.color + 0.6f * Color.red;
    }

    protected override void OnSuccess()
    {
        base.OnSuccess();
        if (player == null) return;
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
    }

}
