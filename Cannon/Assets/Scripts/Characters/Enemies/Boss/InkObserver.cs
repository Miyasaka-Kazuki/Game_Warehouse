using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボス戦プレイヤー強制移動イベントクラス
public class InkObserver : CharacterObserver {
    public Collider[] turnOffColInMotion;
    private float firstSpeed;
    private float movePos;
    private float preMovePos;
    private bool isRegisterd;
    private PlayerStatus playerStat;
    private float firstTimer;
    private float time = 1f;
    private float gravityTimer;
    private float gravity = 2.5f;
    private Animator nowAnim;

	//初期化関数
	void Start () {
        firstSpeed = 2.5f * gravity; //10秒 // v = 0.5*G*t
        preMovePos = 0;
        playerStat = GameObject.Find("Player").GetComponent<PlayerStatus>();
        isRegisterd = false;
        gravityTimer = 0;
	}

	//更新関数
	void Update () {
        if (!isRegisterd) return;
        firstTimer += Time.deltaTime;
        //地面についたら終了
 		if (playerStat.GetIsGrounded() && firstTimer > time) {
            isRegisterd = false;
            for (int i = 0; i < turnOffColInMotion.Length; i++) {
                turnOffColInMotion[i].enabled = true;
            }
            nowAnim.Play("Wait");
            GameDirector.Instance().playerDirector.RemoveObserver(this);
            GetComponent<BossStatus>().SetIsInMotion(false);
        }
	}

    //上に飛び上がる
    public override Vector3 ActivateCharaObserver(ref bool notUsePlayerMove, ref bool notUseWeapon, ref bool notUseLookAt,
        ref bool muteki, ref Vector3 otherLookAt, ref bool useWarp, 
        bool flownDamaged, Vector3 movePosition, Animator anim = null) {

        notUsePlayerMove = true;
        notUseWeapon = true;
        muteki = true;
        nowAnim = anim;
        anim.Play("JumpOfTop");
        float nowSpeed = firstSpeed - gravity * Time.fixedDeltaTime;
        firstSpeed = nowSpeed;
        return nowSpeed * transform.up * Time.fixedDeltaTime;
    }

    public void AddCharaOb() {
        for (int i = 0; i < turnOffColInMotion.Length; i++) {
            turnOffColInMotion[i].enabled = false;
        }
        isRegisterd = true;
        GameDirector.Instance().playerDirector.AddObserver(this);
    }
}
