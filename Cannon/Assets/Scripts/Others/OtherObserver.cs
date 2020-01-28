using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの状態に上書きまたは足し算するための他オブジェクト更新クラスのベースクラス
public abstract class OtherObserver : BaseObserver {
    public override Vector3 ActivateOtherObserver(ref bool notUseMovePos, ref bool notUseLookAt,
        ref Vector3 lookAtPos, Vector3 movePosition = new Vector3(),
        Animator anim = null) { return Vector3.zero; }

    public override void Enter(GameObject otherObj) { }

    public override void Exit(GameObject otherObj) { }
}
