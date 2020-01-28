using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー移動速度低下クラス
public class InkSkate : CharacterObserver {
    private PlayerStatus status;
    private bool preRun;
    private float preSpeed;
    private CharacterAdapter charaAda;

    private Vector3 movePos;
    private float slipTime;
    private float slipMaxTime = 1;
    private Vector3 preDirection;
    private int preListCount;
    private int count;

	//初期化関数
	void Start () {
        preRun = false;
        preSpeed = 0;
        preDirection = Vector3.zero;
        charaAda = GetComponent<CharacterAdapter>();
        status = GameDirector.Instance().playerDirector.GetComponent<PlayerStatus>();
        slipTime = 0;
        count = 0;
	}

	//更新関数
	void Update () {
        if (charaAda.GetCurRegisterListCount() > 0) {
            if (preListCount <= 0) { //インク床に入った瞬間
                slipTime = 0;
            }
            preDirection = Vector3.zero;
            charaAda.RegisterCharaDirector();
        }
        preListCount = charaAda.GetCurRegisterListCount();
	}

    //Bossはこれらのオブジェクトをtrueにするだけ
    //BossStageに入れる
    public override Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon, ref bool notUseLookAt,
        ref bool isInMuteki, ref Vector3 otherLookAt, ref bool useWarp, 
        bool flownDamaged, Vector3 movePosition = default(Vector3), Animator anim = null) {

        return -movePosition + movePosition / 3.0f;
    }
}
