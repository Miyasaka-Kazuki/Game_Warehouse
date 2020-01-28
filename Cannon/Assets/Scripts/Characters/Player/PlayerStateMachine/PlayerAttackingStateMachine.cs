using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの攻撃状態遷移管理クラス
public class PlayerAttackingStateMachine : PlayerStateEntry {
    PlayerStateEntry currentState; //現在の状態
    private Animator anim; //アニメーション
    private PlayerStatus status; //ステータス

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        if (upperState.GetPreState() == jumping) {
            currentState = attackingInAir;
        } else {
            currentState = attackingFirst;
        }
        currentState.Enter(this);
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = currentState.Activate(ref lookAtPos);
        currentState.IsChanging(this);

        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        currentState.Exit(this);
    }

    public override void ChangeState(PlayerStateEntry nextState) {
        currentState.Exit(this);
        currentState = nextState;
        currentState.Enter(this);
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float endTime = info.normalizedTime;

        if (status.GetDamaged()) {
            upperState.ChangeState(damaging);
        } else if (endTime >= 0.65f &&
              (info.IsName("Attack1") || info.IsName("Attack2") || info.IsName("Attack3")))
            upperState.ChangeState(upperState.GetPreState());
    }
}
