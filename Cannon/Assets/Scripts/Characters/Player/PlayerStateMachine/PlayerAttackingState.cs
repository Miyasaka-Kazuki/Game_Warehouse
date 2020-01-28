using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//直接攻撃の１回目状態クラス
public class AttackingFirst : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("Attack1", true);
        attackingFirstCommand.Enter();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = attackingFirstCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("Attack1", false);
        attackingFirstCommand.Exit();
    }

    public override void IsChanging(PlayerStateEntry se) {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float currentAnimTime = info.normalizedTime;
        if (Input.GetButtonDown("Attack") &&
            info.IsName("Attack1")) {
            se.ChangeState(attackingSecond);
        }
    }
}

//直接攻撃の２回目状態クラス
public class AttackingSecond : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("Attack2", true);
        attackingSecondCommand.Enter();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = attackingSecondCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("Attack2", false);
        attackingSecondCommand.Exit();
    }

    public override void IsChanging(PlayerStateEntry se) {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float currentAnimTime = info.normalizedTime;
        if (Input.GetButtonDown("Attack") &&
            info.IsName("Attack2")) {
            se.ChangeState(attackingThird);
        }
    }
}

//直接攻撃の３回目状態クラス
public class AttackingThird : PlayerStateEntry {
    private Animator anim;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        attackingThirdCommand.Enter();
        anim.SetBool("Attack3", true);
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = attackingThirdCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("Attack3", false);
        attackingThirdCommand.Exit();
    }
}

//空中攻撃状態クラス
public class AttackingInAirlial : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = attackingInAirCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (status.GetIsGrounded()) {
            upperState.ChangeState(moving);
        }
    }
}