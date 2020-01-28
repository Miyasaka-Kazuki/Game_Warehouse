using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミーの弾クラス
public class Bullet : BulletBase {
    public float lifeTime;
    public float speed;
    public int attackValue = 1;
    private Rigidbody rigid;
    private bool bumped;

	//初期化関数
    void Start () {
        rigid = GetComponent<Rigidbody>();
        bumped = false;
        bulletValue = attackValue;
	}

	//更新関数
    public override Vector3 Activate() {
        Vector3 movePos = Vector3.zero;
        movePos = transform.forward * speed * Time.fixedDeltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0 || bumped)
            Destroy(this.gameObject);

        return movePos;
    }

	//接触開始時に呼ばれる関数
    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.GetComponent<Collider>().isTrigger) return;

        if (!(col.gameObject.tag == "Weapon" || col.gameObject.tag == "EnemyWeapon")) {
            if (gameObject.tag == "Weapon" && col.tag != "Player")
                bumped = true;
            if (gameObject.tag == "EnemyWeapon" && !(col.tag == "Enemy" || col.gameObject.layer == LayerMask.NameToLayer("Enemy")))
                bumped = true;
        }
    }
}
