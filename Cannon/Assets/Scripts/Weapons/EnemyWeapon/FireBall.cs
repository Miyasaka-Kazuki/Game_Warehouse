using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスの弾クラス
public class FireBall : BulletBase {
    public float lifeTime; //寿命
    public float speed;

	//初期化関数
	void Start() {
        bulletValue = 1;
    }

	//更新関数
    public override Vector3 Activate() {
        Vector3 movePos = Vector3.zero;
        movePos = transform.forward * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(this.gameObject);

        return movePos;
    }
}
