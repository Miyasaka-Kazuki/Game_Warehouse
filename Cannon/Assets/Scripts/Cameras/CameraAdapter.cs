using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラの状態に上書きまたは足し算するための仲介クラス
public class CameraAdapter : MonoBehaviour {
    CameraObserver cameraOb;
    CameraDirector cameraDirector;
    bool isPlayer;

	//初期化処理関数
    void Start() {
        cameraOb = GetComponent<CameraObserver>();
        cameraDirector = Camera.main.GetComponent<CameraDirector>();
        isPlayer = false;
    }

	//プレイヤーがカメラ変更点に接触した瞬間
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
            isPlayer = true;
    }

	//プレイヤーがカメラ変更点から離れた瞬間
	void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            isPlayer = false;
            cameraDirector.RemoveObserver(this.cameraOb);
        }
    }

	//他カメラを登録
    public void RegisterCameraDirector() {
        cameraDirector.AddObserver(this.cameraOb);
    }

	//他カメラを削除
    public void ReleaseCameraDirector() {
        cameraDirector.RemoveObserver(this.cameraOb);
    }


    public bool IsPlayer() {
        return isPlayer;
    }
}
