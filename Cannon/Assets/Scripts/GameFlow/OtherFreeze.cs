using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//その他オブジェクトを停止させるクラス
public class OtherFreeze : OtherObserver {
    private SortedList<Animator, float> animList;
    private Vector3 lookAtPos;

	//初期化関数
    public void Start() {        
        animList = new SortedList<Animator, float>();
    }
		
    public override void Enter(GameObject otherObj) {
        Animator anim = otherObj.GetComponent<Animator>();
        if (anim != null && anim.enabled)
            animList.Add(anim, anim.speed);
    }
		
    public override Vector3 ActivateOtherObserver(ref bool notUseMovePos, ref bool notUseLookAt, ref Vector3 lookAtPos, Vector3 movePosition = default(Vector3), Animator anim = null) {
        notUseMovePos = true;
        notUseLookAt = true;

        if (anim != null)
            anim.speed = 0;

        return Vector3.zero;
    }

    public override void Exit(GameObject otherObj) {

        for (int i = animList.Count - 1; i >= 0; i--) {
            if (animList.Keys[i] == null) continue;
            animList.Keys[i].speed = animList.Values[i];
        }
        animList.Clear();
    }
}
