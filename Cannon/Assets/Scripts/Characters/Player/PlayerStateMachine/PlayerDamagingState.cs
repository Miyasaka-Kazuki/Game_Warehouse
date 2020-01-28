using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//攻撃を受けた時のノックバック状態クラス
public class NockBacking : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("Damage", true);
        nockBackingCommand.Enter();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = nockBackingCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("Damage", false);
        nockBackingCommand.Exit();
    }
}

//死亡状態クラス
public class Dying : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetTrigger("Dead");
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = dyingCommand.Activate(ref lookAtPos);
        return movePosition;
    }
}
