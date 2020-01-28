using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//移動床ギミッククラス
public class MoveFloor : OtherMoveMachine {
    public float max_distance = 5;
    public bool isHeight = false;
    public float initTheta = 0;
    private Vector3 movePos;
    private Vector3 preOffset;
    private Vector3 basePosition;
    private float theta;
    private CharacterAdapter charaAda;

    //初期化関数
    void Start () {
        movePos = Vector3.zero;
        preOffset = Vector3.zero;
        basePosition = transform.position;
        theta = initTheta;
        charaAda = GetComponent<CharacterAdapter>();
	}
		
	//更新関数
    public override Vector3 Activate() {
        Vector3 direction = transform.right;
        if (isHeight)
            direction = transform.up;

        //軸に沿って動く
        Vector3 curOffset = direction * max_distance * Mathf.Sin(theta * Mathf.Deg2Rad);
        movePos = curOffset - preOffset;
        //        transform.position += movePos;

        preOffset = curOffset;
        theta++;
        if (theta >= 360)
            theta -= 360;

        return movePos;
    }

    public Vector3 GetMovePos() {
        return movePos;
    }
}
