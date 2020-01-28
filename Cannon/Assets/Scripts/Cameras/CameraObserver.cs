using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//カメラの状態に上書きまたは足し算するための他オブジェクト更新クラス
public abstract class CameraObserver : BaseObserver {
    public override Vector3 ActivateCameraObserver(ref bool notUseLookAt, ref Vector3 lookAtPosition) {
        return Vector3.zero;
    }
    public override void Enter(GameObject callObj) { }
    public override void Exit(GameObject callObj) { }
}
