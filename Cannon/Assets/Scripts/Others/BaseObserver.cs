using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの状態に上書きまたは足し算するための他オブジェクト更新クラスのベースクラス
public abstract class BaseObserver : MonoBehaviour {
    public abstract void Enter(GameObject callObj); //更新開始に呼ばれる関数
    public abstract void Exit(GameObject callObj); //更新終了に呼ばれる関数

	//それぞれの更新関数
    //character用
    public virtual Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon,
    ref bool notUseLookAt, ref bool isInMuteki, ref Vector3 otherLookAt, ref bool useWarp,
    bool flownDamaged, Vector3 movePosition = new Vector3(), Animator anim = null) {
        return Vector3.zero;
    }
    //Camera用
    public virtual Vector3 ActivateCameraObserver(ref bool notUseLookAt, ref Vector3 lookAtPosition) {
        return Vector3.zero;
    }
    //other用
    public virtual Vector3 ActivateOtherObserver(ref bool notUseMovePos, ref bool notUseLookAt,
        ref Vector3 lookAtPos, Vector3 movePosition = new Vector3(),
        Animator anim = null) { return Vector3.zero; }
}
