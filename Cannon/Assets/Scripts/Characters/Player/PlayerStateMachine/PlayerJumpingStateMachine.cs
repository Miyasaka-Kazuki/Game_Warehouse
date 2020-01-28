using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーのジャンプ状態遷移管理クラス
public class PlayerJumpingStateMachine : PlayerStateEntry {
    private PlayerStateEntry currentState; //現在の状態
    private PlayerStateEntry preState; //前回の状態
    private PlayerStatus status; //ステータス
    private float waitingTimer; //状態遷移の待機時間
    private const float thresholdTime = 0.1f;
    private Animator anim;


    protected override void SubInitialize(GameObject usingObj) {

        status = usingObj.GetComponent<PlayerStatus>();
        anim = usingObj.GetComponent<Animator>();
    }


    public override void Enter(PlayerStateEntry upperState) {

        //if (status.GetSlideButton()) {
        //    currentState = slideJumping;
        /*} else*/ if (status.GetJumpButton()) {
            currentState = jumpingFirst;
        } else {
            currentState = jumpingWithUpToDown;
        }

        currentState.Enter(this);
        preState = currentState;

        waitingTimer = 0;        
    }


    public override Vector3 Activate(ref Vector3 lookAtPos) {

        waitingTimer += Time.fixedDeltaTime;

        Vector3 movePosition = Vector3.zero;
        movePosition = currentState.Activate(ref lookAtPos);
        currentState.IsChanging(this);

        return movePosition;
    }


    public override void Exit(PlayerStateEntry upperState) {

        currentState.Exit(this);

        //アニメーション処理
        if (currentState == wallJumping) {
            anim.SetBool("JumpOfTop", false);
        }
    }


    public override void ChangeState(PlayerStateEntry nextState) {

        currentState.Exit(this);
        preState = currentState;
        currentState = nextState;
        currentState.Enter(this);
    }


    public override void IsChanging(PlayerStateEntry upperState) {

        if (status.GetDamaged()) {
            upperState.ChangeState(damaging);
            //waitingTimerがあるのはすぐにwaitingに移ってしまうため
        } else if (status.GetIsGrounded() && waitingTimer > thresholdTime) {
            upperState.ChangeState(moving);
        }
    }


    public override PlayerStateEntry GetPreState() {

        return preState;
    }
}