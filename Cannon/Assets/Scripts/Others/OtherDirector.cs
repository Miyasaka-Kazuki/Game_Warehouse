using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//その他オブジェクト全体を管理するクラス
public class OtherDirector : BaseDirector {
    private Animator anim;
    private OtherMoveMachine moveMachine;
    private bool freezed;

	//初期化関数
	public void Start() {
        type = DirectorType.Other;
        baseObserver = new List<BaseObserver>();
        anim = GetComponent<Animator>();
        moveMachine = GetComponent<OtherMoveMachine>();

        freezed = false;
    }
		
	//更新関数
    private void Update() {        
        GameDirector.Instance().AddOtherDirector(this);
    }
		
	//更新関数
    public void Activate() {
        Vector3 movePos = Vector3.zero;
        Vector3 lookAtPos = transform.position + transform.forward;

        if (!freezed && moveMachine != null)
            movePos = moveMachine.Activate();

        OtherMovePosition(movePos, lookAtPos);

        transform.position += movePos;
    }

	//オブジェクト状態を上書きまたは足し算で制限する関数
    private Vector3 OtherMovePosition(Vector3 movePosition, Vector3 lookAtPos) {
        //ギミックなどでプレイヤーの動きを制限する
        Vector3 otherMove = Vector3.zero;
        Vector3 otherLookAt = transform.position + transform.forward;
        bool notUseMovePos = false;
        bool notUseLookAt = false;

        foreach (BaseObserver oo in baseObserver) {
            bool nUMP = false;
            bool nULA = false;
            otherMove += oo.ActivateOtherObserver(ref nUMP, ref nULA, ref otherLookAt, movePosition, anim);
            if (nUMP) notUseMovePos = true;
            if (nULA) notUseLookAt = true;
        }

        if (notUseMovePos) {
            movePosition = otherMove;
            freezed = true;
        } else {
            movePosition += otherMove;
            freezed = false;
        }

        if (notUseLookAt)
            lookAtPos = otherLookAt;

        return movePosition;
    }
}
