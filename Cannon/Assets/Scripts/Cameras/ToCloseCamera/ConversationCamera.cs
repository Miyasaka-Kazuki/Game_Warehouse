using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//掲示板を見るときの固定カメラ
public class ConversationCamera : CameraObserver {
    private SignBord signBord; //掲示板オブジェクト
    private Transform model; //掲示板座標

	//初期化処理
	void Start () {
        signBord = GetComponent<SignBord>();
        model = transform.GetChild(0);
	}

	//更新関数
    private void Update() {
        if (signBord.GetIsInSentence())
            Camera.main.GetComponent<CameraDirector>().AddObserver(this);
        else
            Camera.main.GetComponent<CameraDirector>().RemoveObserver(this);
    }

	//カメラ座標と視点の更新関数
    public override Vector3 ActivateCameraObserver(ref bool notUseCameraLookAt,  ref Vector3 lookAtPosition) {
        Transform player = GameDirector.Instance().playerDirector.transform;

        Vector3 centerPos = (player.position + model.position) / 2.0f;
        Vector3 toPlayer = player.position - model.position;
        toPlayer.y = 0;
        Vector3 toCamera = Camera.main.transform.position - model.position;
        toCamera.y = 0;

        float angle = 90;
        Vector3 cameraOffsetPos = Quaternion.Euler(0, angle, 0) * toPlayer.normalized;

        notUseCameraLookAt = true;
        lookAtPosition = Camera.main.transform.position + Quaternion.Euler(0, -angle, 0) * toPlayer.normalized;
        return centerPos + cameraOffsetPos * 2;
    }
}
