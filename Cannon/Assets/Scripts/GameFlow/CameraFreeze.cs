using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラをフリーズさせるクラス
public class CameraFreeze : CameraObserver {
    private Vector3 lookAtPos;

    public override void Enter(GameObject callObj) {
        lookAtPos = Camera.main.transform.position + Camera.main.transform.forward;
    }

    public override Vector3 ActivateCameraObserver(ref bool notUseCameraLookAt,  ref Vector3 lookAtPosition) {

        notUseCameraLookAt = true;
        lookAtPosition = lookAtPos;
        return Camera.main.transform.position;
    }

    public override void Exit(GameObject callObj) {
    }
}
