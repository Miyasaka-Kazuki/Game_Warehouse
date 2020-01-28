using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラクターを停止させるクラス
public class CharacterFreeze : CharacterObserver {
    private Vector3 playerLookAtPos;
    private List<Animator> charaAnimList;

	//初期化関数
    private void Start() {
        charaAnimList = new List<Animator>();
    }
		
    public override void Enter(GameObject charaObj) {
        if (charaObj.tag == "Player")
            playerLookAtPos = charaObj.transform.forward;
        charaAnimList.Add(charaObj.GetComponent<Animator>());
    }

    public override Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon, ref bool notUseLookAt,
        ref bool isInMuteki, ref Vector3 otherLookAt, ref bool useWarp, 
        bool flownDamaged, Vector3 movePosition = default(Vector3), Animator anim = null) {

        //フリーズ
        notUsePlayerMove = true;
        notUseWeapon = true;
        notUseLookAt = true;
        isInMuteki = true;
        anim.speed = 0; //止める

        //playerのLookAtだけやればいい
        if (anim.gameObject.tag == "Player") {
            otherLookAt = anim.transform.position + playerLookAtPos;
        }

        return Vector3.zero;
    }
		
    public override void Exit(GameObject charaObj) {

        for (int i = charaAnimList.Count-1; i >= 0; i--) {
            if (charaAnimList[i] == null) continue;
            charaAnimList[i].speed = 1;
        }
        charaAnimList.Clear();
    }
}
