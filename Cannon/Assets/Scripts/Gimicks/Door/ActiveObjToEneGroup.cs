using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//周辺エネミーを全員倒すとドアが開くギミッククラス
public class ActiveObjToEneGroup : MonoBehaviour {
    public Animator doorAnim;
    bool isEnemyEntered;
    bool isPreEnemyEntered;
    bool isPlayerEntered;
    bool isEnded;
    bool isActivatedObj;
    List<GameObject> enemies;

	//初期化関数
	void Start () {
        doorAnim.speed = 0;
        isEnemyEntered = false;
        isPreEnemyEntered = false;
        isPlayerEntered = false;
        isActivatedObj = false;
        isEnded = false;
        enemies = new List<GameObject>();
	}
	
	//更新関数
	void Update () {
        //EnemyGroupの子オブジェクトが全部なくなったら
        for (int i = enemies.Count - 1; i >= 0; i--) {
            if (enemies[i] == null)
                enemies.RemoveAt(i);
        }
        if (enemies.Count == 0)
            isEnemyEntered = false;
        else
            isEnemyEntered = true;

        //敵がいなくなって、プレイヤーが中にいるとき
        if (!isPlayerEntered || isEnded) return;
        if (!(isPreEnemyEntered && !isEnemyEntered)) {
            isActivatedObj = true;
        }
        if (!isActivatedObj) return;

        //全員倒されたら
        if (enemies.Count == 0) {
            //カメラ実行
            Camera.main.GetComponent<CameraDirector>().
                AddObserver(GetComponent<AttentionCamera>());

            //プレイヤーフリーズ
            GameDirector.Instance().PlayerFreeze();

            //Doorアニメーションを実行
            doorAnim.speed = 0.5f;
            doorAnim.Play("DoorAnim");
            doorAnim.gameObject.GetComponent<BoxCollider>().enabled = false;

            float animTime = doorAnim.GetCurrentAnimatorStateInfo(0)
                .normalizedTime;
            if (animTime >= 1) {
                isEnded = true;
                Camera.main.GetComponent<CameraDirector>().
                    RemoveObserver(GetComponent<AttentionCamera>());
                GameDirector.Instance().ReleasePlayerFreeze();
            }
        }

        isPreEnemyEntered = isEnemyEntered;
	}

	//非物理接触の開始関数
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            enemies.Add(other.gameObject);
        } else if (other.tag == "Player") {
                isPlayerEntered = true;
        }
    }

	//非物理接触の終了関数
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            isPlayerEntered = false;
        }
    }
}
