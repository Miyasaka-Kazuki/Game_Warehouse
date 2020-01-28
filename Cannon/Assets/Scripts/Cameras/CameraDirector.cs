using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラ全体を管理するクラス
public class CameraDirector : BaseDirector {
    PlayerCamera mainCamera; //メインカメラ
    Vector3 nextPosition; //次の座標
    Vector3 lookAtPosition; //視点

	//初期化関数
	void Start () {
        type = DirectorType.Camera;
        mainCamera = GetComponent<PlayerCamera>();
        baseObserver = new List<BaseObserver>(3);
        nextPosition = Vector3.zero;
        lookAtPosition = Vector3.zero;
	}

	//更新関数
    public void Activate () {
        if (baseObserver.Count == 0)
            nextPosition = mainCamera.Activate(ref lookAtPosition);

        OtherMovePosition(); //他のカメラの動きを優先する

        transform.LookAt(lookAtPosition);
        transform.position = nextPosition;
	}

    //カメラ状態を上書きまたは足し算で制限する関数
    private void OtherMovePosition() {

        if (baseObserver.Count == 0) return;

        bool notUseCameraLookAt = false;
        Vector3 lookAtPos = transform.position + transform.forward;
        nextPosition = baseObserver[baseObserver.Count-1].ActivateCameraObserver(ref notUseCameraLookAt, ref lookAtPos);
        if (notUseCameraLookAt) lookAtPosition = lookAtPos;
    }
}
