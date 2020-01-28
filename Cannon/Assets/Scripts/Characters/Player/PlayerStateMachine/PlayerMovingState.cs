using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//待機状態クラス
public class Waiting : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        waitingCommand.Enter();
        anim.SetBool("JumpOfTop", false);
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = waitingCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (status.GetRunButton()) {
            upperState.ChangeState(running);
        }
    }
}

//移動状態クラス
public class Running : PlayerStateEntry {
    private Animator anim;
    private PlayerStatus status;

    protected override void SubInitialize(GameObject usingObj) {
        anim = usingObj.GetComponent<Animator>();
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter(PlayerStateEntry upperState) {
        anim.SetBool("Running", true);
        runningCommand.Enter();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        movePosition = runningCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void Exit(PlayerStateEntry upperState) {
        anim.SetBool("Running", false);
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (!status.GetRunButton())
            upperState.ChangeState(waiting);
    }
}

//横移動状態クラス
public class SlideRunning : PlayerStateEntry {
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
        movePosition = slideRunningCommand.Activate(ref lookAtPos);
        return movePosition;
    }

    public override void IsChanging(PlayerStateEntry upperState) {
        if (!status.GetSlideButton())
            upperState.ChangeState(running);
        else if (!status.GetRunButton()) {
            upperState.ChangeState(waiting);
        }
    }
}