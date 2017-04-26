using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTask : LockOutAllInput {

    public StunTask(float dur, Player pl) : base(dur, pl) { }

    protected override void Init()
    {
        if (player == null) return;
        base.Init();
        player.stun = true;
        player.gameObject.GetComponent<SpriteRenderer>().color = Services.GameManager.stunColors[player.playerNum - 1];/*0.4f * player.color + 0.6f * Color.red;*/
    }

    protected override void OnSuccess()
    {
        if (player == null) return;
        base.OnSuccess();
        player.stun = false;
        player.gameObject.GetComponent<SpriteRenderer>().color = player.color;
    }

}
