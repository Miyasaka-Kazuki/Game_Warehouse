using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーの行動状態遷移管理クラス
public class PlayerStateMachine : PlayerStateEntry {
    PlayerStateEntry currentState; //現在の状態
    PlayerStateEntry preState; //前フレームの状態

	//初期化
    protected override void SubInitialize(GameObject usingObj) {
        currentState = moving;
        preState = moving;
        currentState.Enter(this);
    }

	//更新関数
    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = currentState.Activate(ref lookAtPos);
        currentState.IsChanging(this);
        return movePosition;
    }

	//状態遷移関数
    public override void ChangeState(PlayerStateEntry se) {
        currentState.Exit(this);
        preState = currentState;
        currentState = se;
        currentState.Enter(this);
    }

    public override PlayerStateEntry GetPreState() {
        return preState;
    }
}
