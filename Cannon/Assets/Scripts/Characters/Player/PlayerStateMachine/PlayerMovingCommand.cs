using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//待機コマンド
public class WaitingCommand : PlayerCommandBase {
    private PlayerStatus status;
    private Vector3 movePosition;
    private float acceleration;

    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponent<PlayerStatus>();
    }

    public override void Enter() {
        acceleration = 0;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        movePosition.y += (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        acceleration -= status.GetGravity();
        if (status.GetIsGrounded())
            movePosition = Vector3.zero;
        return movePosition;
    }
}

//移動コマンド
public class RunningCommand : PlayerCommandBase {
    private Transform activeCamera;
    private PlayerStatus status;
    private float acceleration;

    public void Start() {
        activeCamera = Camera.main.transform;
        status = GetComponent<PlayerStatus>();
    }

    public override void Enter() {
        acceleration = 0;
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //入力取得
        float x = status.GetHorizontalInput();
        float z = status.GetVerticalInput();
        //カメラの向きにプレイヤーを動かす
        //カメラの方向からX-Z平面の単位ベクトルを取得(各成分を乗算して単位ベクトルにする)
        Vector3 cameraForward = Vector3.Scale(
            activeCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = x * activeCamera.right + z * cameraForward;
        moveDirection.y = 0;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        float fallDis = (acceleration * Time.fixedDeltaTime) * Time.fixedDeltaTime;
        acceleration -= status.GetGravity();
        if (!status.GetIsGrounded())
            movePosition.y = fallDis;

        lookAtPos = transform.position + moveDirection;
        return movePosition;
    }
}

//横移動コマンド
public class SlideRunningCommand : PlayerCommandBase {
    private Transform activeCamera;
    private PlayerStatus status;

    public void Start() {
        activeCamera = Camera.main.transform;
        status = GetComponent<PlayerStatus>();
    }

    public override Vector3 Activate(ref Vector3 lookAtPos) {
        Vector3 movePosition = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;

        //入力取得
        float x = status.GetHorizontalInput();
        //カメラの向きにプレイヤーを動かす
        //カメラの方向からX-Z平面の単位ベクトルを取得(各成分を乗算して単位ベクトルにする)
        moveDirection = x * activeCamera.right;
        moveDirection.y = 0;

        movePosition += moveDirection * status.GetMoveSpeed() * Time.fixedDeltaTime;

        Vector3 lookAtDirection = activeCamera.forward;
        lookAtDirection.y = 0;

        lookAtPos = transform.position + lookAtDirection;
        return movePosition;
    }
}