using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//掲示板のバルーンアイコンUIクラス
public class BalloonIconUI : BaseUI {
    private Collider model;

	//初期化関数
    public override void Initialize(GameObject callObj) {
        model = callObj.GetComponentInChildren<Collider>();
        transform.parent = callObj.transform;
    }

	//更新関数
    public override void ActivateUI() {
        Vector3 extence = model.bounds.extents;
        extence.x = 0;
        extence.z = 0;

        //内部で、(指定したposition-自身のposition)をしているからこうしないとダメ
        //Quad,Planeはカメラと逆向きにしなければ見えない
        transform.LookAt(transform.position + Camera.main.transform.forward);

        Vector3 toCamera = Camera.main.transform.position - transform.position;
        toCamera.y = 0;
        transform.position = model.bounds.center + extence * 0.4f + toCamera.normalized;
    }
}
