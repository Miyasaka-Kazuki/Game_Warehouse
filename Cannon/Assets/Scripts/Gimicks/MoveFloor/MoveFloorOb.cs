using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー移動のための仲介クラス
public class MoveFloorOb : CharacterObserver {
    private CharacterAdapter charaAda;
    private Vector3 movePos;

    //初期化関数
    void Start() {
        charaAda = GetComponent<CharacterAdapter>();
    }

    //更新関数
    void FixedUpdate() {
        if (charaAda.GetCurRegisterListCount() > 0)
            charaAda.RegisterCharaDirector();
    }

	//キャラクター更新関数
    public override Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon, ref bool notUseLookAt,
        ref bool isInMuteki, ref Vector3 otherLookAt, ref bool useWarp,
        bool flownDamaged, Vector3 movePosition = default(Vector3), Animator anim = null) {

        movePos = GetComponent<MoveFloor>().GetMovePos();
        return movePos;
    }
}
