using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Property {
    Hide, //隠れる
}

//ステージに設置
//近くにいるエネミーの行動を上書きするクラス
public class AffordanceTrigger : MonoBehaviour {
    public Property property;  //種類の変数
    public Transform planeDestination; //目的地
    private Vector3 destination; //目的座標
    private bool isUsed; //使われているか
    private GameObject enemy; //エネミーオブジェクト

	//初期化処理
    void Start () {
        isUsed = false;
        enemy = null;

        RaycastHit hit;
        bool b = Physics.Raycast(planeDestination.position, Vector3.down, out hit, 
            Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore);
                destination = hit.point;
	}

	//更新関数
    public void Update() {
        if (enemy == null)
            isUsed = false;
    }

	//目的地の取得関数
    public Vector3 GetDestination() {
        return destination;
    }

	//種類の取得関数
    public bool IsToMatchProperty(Property pro) {
        return property == pro;
    }

	//使用されているかの取得関数
    public bool IsUsed() {
        return isUsed;
    }

	//使用されているかのセット関数
    public void SetIsUsed(bool b) {
        isUsed = b;
    }

	//エネミーを設定する関数
    public void SetEnemy(GameObject ene) {
        enemy = ene;
    }

	//接触開始関数
    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            other.GetComponent<EnemyStatus>().AddAfforTriObserver(this);
        }
    }

	//接触終了関数
    public void OnTriggerExit(Collider other) {
        if (other.tag == "Enemy") {
            other.GetComponent<EnemyStatus>().RemoveAfforTriObserver(this);
        }
    }
}
