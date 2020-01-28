using UnityEngine;
using System.Collections;

//親オブジェクトに非物理接触判定を報告するクラス
public class InformParentTrigger : MonoBehaviour {
    private ParentTrigger parentTrigger;

    //初期化関数
    void Start() {
        parentTrigger = transform.root.GetComponent<ParentTrigger>();
        if (parentTrigger == null)
            Debug.Log("<color=red>parentTriggerが無いよ！ : " + transform.root.gameObject + "</color>");
    }

	//非物理接触開始時の関数
    void OnTriggerEnter(Collider collider) {
        parentTrigger.TriggerOnParent(collider);
    }
}