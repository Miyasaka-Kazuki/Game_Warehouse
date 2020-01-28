using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーのダメージ行動状態遷移管理クラス
public class PlayerDamagingStateMachine : PlayerStateEntry {
    PlayerStateEntry currentState; //現在の状態
    private Animator anim; //アニメーション
    private PlayerStatus status; //ステータス

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        if (status.GetDead())
            currentState = dying;
        else
            currentState = nockBacking;

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
        float currentDamagedTime = info.normalizedTime;
        bool curNockBack = (currentState == nockBacking);
        if (currentDamagedTime >= 0.5f && status.GetIsGrounded()) {
            if (info.IsTag("NockBack")) {
                status.SetDamaged(false);
                upperState.ChangeState(moving);
            }
        }
    }
}
