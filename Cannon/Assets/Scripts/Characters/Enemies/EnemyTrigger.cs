using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミグループのインスタンス判定クラス
public class EnemyTrigger : MonoBehaviour {
    public string wantEnemyGroup;
    public Transform insPos;

    private void OnTriggerEnter(Collider other) {
        //生成
        if (other.gameObject.tag == "Player") {
            transform.root.GetComponent<MetaAI>().InstanceEnemy(wantEnemyGroup, insPos.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        //削除
        if (other.gameObject.tag == "Player") {
            transform.root.GetComponent<MetaAI>().DestroyEnemy(insPos.gameObject);
        }
    }
}
