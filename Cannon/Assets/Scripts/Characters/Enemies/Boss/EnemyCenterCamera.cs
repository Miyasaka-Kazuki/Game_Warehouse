using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敵中心視点カメラクラス
public class EnemyCenterCamera : CameraObserver {
    public float distanceFromCenter = 10;
    public float height = 5;
    private CameraDirector cameraDire;
    private BossStatus status;
    private bool first;

	//初期化関数
	void Start () {
        cameraDire = Camera.main.GetComponent<CameraDirector>();
        status = GetComponent<BossStatus>();
        first = true;
	}

	//更新関数
	void Update () {
        if (first) {
            first = false;
            cameraDire.AddObserver(this);
        }
	}

    public override Vector3 ActivateCameraObserver(ref bool notUseCameraLookAt,  ref Vector3 lookAtPosition) {
        Vector3 lookVec = status.GetFootCenter().position - status.GetPlayer().position;
        if (!status.GetIsInMotion()) {
            lookVec.y = 0;
        }
        lookVec += Camera.main.transform.position;

        notUseCameraLookAt = true;
        lookAtPosition = lookVec;

        float nowAngle = Camera.main.transform.rotation.eulerAngles.y;
        Vector3 position;
        position.z = distanceFromCenter * Mathf.Cos(nowAngle * Mathf.Deg2Rad);
        position.x = distanceFromCenter * Mathf.Sin(nowAngle * Mathf.Deg2Rad);

        if (!status.GetIsInMotion()) {

            position.y = height;
        } else {

            position.y = status.GetPlayer().position.y + height;
        }

        return status.GetFootCenter().position + position;

    }
}
