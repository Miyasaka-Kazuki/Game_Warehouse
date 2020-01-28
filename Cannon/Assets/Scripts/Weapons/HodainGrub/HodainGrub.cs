using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器砲台クラス
public class HodainGrub : BulletBase {
    public float lifeTime; //寿命
    public GameObject bulletPrefab; //バレットプレハブ
    public Transform bulletPosition; //バレットポジション

    private float intervalTimer; //弾間のインターバルタイマー
    private float thresholdTime = 0.5f; //閾値
    private List<Transform> searchEnemyList; //周辺のエネミーリスト

	//初期化関数
    public void Initialize() {
        bulletValue = 1;
        searchEnemyList = new List<Transform>();
    }

	//更新関数
	public override Vector3 Activate() {
        intervalTimer += Time.fixedDeltaTime;
        Transform lockOnEnemy = ComputeLockOnEnemy();
        if (intervalTimer > thresholdTime && lockOnEnemy != null) {
            intervalTimer = 0;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, transform.rotation/*,  Quaternion.Euler(thisVec)*/);

            bullet.transform.LookAt(lockOnEnemy.position + Vector3.up * 0.5f);
        }

		//ロックオンしている敵がいるならそれに向ける
        if (lockOnEnemy != null) {
            Vector3 vecToEne = lockOnEnemy.position - transform.position;
            vecToEne.y = 0;

            transform.LookAt(transform.position + vecToEne);
        }

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(this.gameObject);

        return Vector3.zero;
    }

	//周辺エネミー追加関数
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy" || other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            searchEnemyList.Add(other.transform);
        }
    }

	//周辺エネミー削除関数
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Enemy" || other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            searchEnemyList.Remove(other.transform);
        }
    }

    //狙える敵がいたらTransformを,いなかったらNULLを返す関数
    public Transform ComputeLockOnEnemy() {
        //デストロイされているかどうか調べる
        for (int i = searchEnemyList.Count - 1; i >= 0; i--) {
            if (searchEnemyList[i] == null)
                searchEnemyList.RemoveAt(i);
        }
        if (searchEnemyList.Count == 0) return null;

        //プレイヤーとの距離のソート
        SortedList<float, Transform> dis_ene_map = new SortedList<float, Transform>();
        foreach (Transform enemy in searchEnemyList) {
            float toPlayer = (transform.root.position - enemy.position).magnitude;
            dis_ene_map[toPlayer] = enemy;
        }
        return dis_ene_map.Values[0];
    }
}
