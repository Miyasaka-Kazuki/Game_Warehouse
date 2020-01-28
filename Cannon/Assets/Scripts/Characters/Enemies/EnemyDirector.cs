using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミーを管理するクラス
public class EnemyDirector : CharacterDirector {
    private BaseEnemyTree behaviorTree; //エネミーの行動管理クラス
    private EnemyStatus status; //エネミーステータス
    private CharacterController charCon; //移動管理
    private Vector3 movePosition; //1フレームの偏移距離
    private Animator anim; //アニメーション
    private bool useWarp; //ワープ移動できるか

	//初期化処理関数
    void Start() {
        type = DirectorType.Character;
        behaviorTree = GetComponent<BaseEnemyTree>();
        charCon = GetComponent<CharacterController>();
        status = GetComponent<EnemyStatus>();
        baseObserver = new List<BaseObserver>(10);
        movePosition = Vector3.zero;
        anim = GetComponent<Animator>();
        useWarp = false;
    }

	//更新関数
    public void Activate() {
        useWarp = false;

        Vector3 lookAtPos = transform.position + transform.forward;
        movePosition = behaviorTree.Activate(ref lookAtPos);

        //ダメージを受けた時、赤く点滅させる
        if (status.GetDamaged()) {
            status.DamageExpression();
        }

        //他のMoveで制限
        if (status.GetHealth() > 0)
            OtherMovePosition(lookAtPos);

        //ここでまとめてやる
        transform.LookAt(lookAtPos);

        if (useWarp)
            transform.position = movePosition;
        else
            charCon.Move(movePosition);
    }

	//他オブジェクトによるエネミーの状態を制限する関数
    private void OtherMovePosition(Vector3 lookAtPos) {
        //ギミックなどでエネミーの動きを制限する
        Vector3 otherMove = Vector3.zero;
        Vector3 otherLookAt = transform.position + transform.forward;
        bool notUseCharacterMove = false; 
        bool notUseWeapon = false; 
        bool notUseLookAt = false;
        bool isInMuteki = false;
        bool useWarpInLocal = false;

        foreach (BaseObserver co in baseObserver) {
            bool anyNUPM  = false; //それぞれがcharacterのmoveを使用するか
            bool anyNUW    = false;
            bool anyNULA   = false;
            bool anyMuteki = false;
            bool anyWarp   = false;

            otherMove += co.ActivateCharaObserver(ref anyNUPM, ref anyNUW, 
                ref anyNULA, ref anyMuteki, ref otherLookAt, ref anyWarp, 
                status.GetFlownDamaged(), movePosition, anim);

            if (anyNUPM) notUseCharacterMove = true;
            if (anyNUW) notUseWeapon             = true;
            if (anyNULA) notUseLookAt              = true;
            if (anyMuteki) isInMuteki                 = true;
            if (anyWarp) useWarpInLocal           = true;
        }

        //ひとつでもエネミーの移動を使わないやつがあるならotherMoveのみ使用する
        if (notUseCharacterMove) movePosition = otherMove;
        else movePosition += otherMove;
        if (notUseLookAt) lookAtPos = otherLookAt;
        useWarp = useWarpInLocal;

        status.SetNotUseWeapon(notUseWeapon);
        status.SetMuteki(isInMuteki);
    }
}

