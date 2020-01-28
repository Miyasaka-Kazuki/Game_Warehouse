using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//注視固定カメラ
public class AttentionCamera : CameraObserver {
    public Transform subCamera;
    private float preAttentionTimer;
    private const float thresholdTime = 0.5f;

    public override void Enter(GameObject callObj) {
        preAttentionTimer = 0;
        base.Enter(callObj);
    }
    public override Vector3 ActivateCameraObserver(ref bool notUseCameraLookAt,  ref Vector3 lookAtPosition) {
        notUseCameraLookAt = true;
        lookAtPosition = Camera.main.transform.position + subCamera.forward;
        return subCamera.position;
    }
}
