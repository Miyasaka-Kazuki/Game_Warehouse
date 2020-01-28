using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの移動状態遷移管理クラス
public class PlayerMovingStateMachine : PlayerStateEntry {
    PlayerStateEntry currentState; //現在の状態
    private PlayerStatus status; //ステータス

    protected override void SubInitialize(GameObject usingObj) {
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        currentState = waiting;
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
        if (status.GetDamaged()) {
            upperState.ChangeState(damaging);
        } else if (status.GetJumpButtonDown())
            upperState.ChangeState(jumping);
        else if (status.GetAttackButtonDown())
            upperState.ChangeState(attacking);
        else if (status.GetJumpingOff())
            upperState.ChangeState(jumping);
    }
}
