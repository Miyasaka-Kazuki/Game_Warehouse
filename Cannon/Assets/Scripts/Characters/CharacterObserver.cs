using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラクターの状態に上書きまたは足し算するための他オブジェクト更新クラス
public abstract class CharacterObserver : BaseObserver {
	//実行処理関数
    public override Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon,
        ref bool notUseLookAt, ref bool isInMuteki, ref Vector3 otherLookAt, ref bool useWarp, 
        bool flownDamaged, Vector3 movePosition = new Vector3(), Animator anim = null) {
        return Vector3.zero;
    }
	public override void Enter(GameObject charaObj) { }	//上書きを開始する瞬間に呼ばれる関数
	public override void Exit(GameObject charaObj) { } //上書きを終了する瞬間に呼ばれる関数
}
