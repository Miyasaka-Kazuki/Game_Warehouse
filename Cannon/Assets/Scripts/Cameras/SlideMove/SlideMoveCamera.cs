using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//横移動で使われる視点固定カメラ
public class SlideMoveCamera : CameraObserver {

    [SerializeField] private float distance = 6;
    [SerializeField] private float height = 0.4f;
    [SerializeField] private Transform target;
    private Vector3 fixForward;


    public override void Enter(GameObject callObj) {

        fixForward =  transform.position - Camera.main.transform.position;
        fixForward.y = 0;
        fixForward = fixForward.normalized;
    }


    public override Vector3 ActivateCameraObserver(ref bool notUseCameraLookAt, ref Vector3 lookAtPosition) {
        notUseCameraLookAt = true;
        lookAtPosition = target.position;

        Vector3 movePos = transform.position;
        movePos -= fixForward * distance;
        movePos.y += height;
        return movePos;
    }
}
