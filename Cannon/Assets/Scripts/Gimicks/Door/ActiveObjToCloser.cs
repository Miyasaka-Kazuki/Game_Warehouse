using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ドアアニメーション管理クラス
public class ActiveObjToCloser : MonoBehaviour {
    public Animator doorAnim; //アニメーション
    bool isEnded; //終わったか
    bool can_start; //開始可能か

    //初期化処理関数
    void Start() {
        doorAnim.speed = 0;
        isEnded = false;
        can_start = false;
    }

	//更新関数
    void Update() {
        if (isEnded) return;
        if (!can_start) return;

        //カメラ実行
        Camera.main.GetComponent<CameraDirector>().
            AddObserver(GetComponent<AttentionCamera>());

        //プレイヤーフリーズ
        GameDirector.Instance().PlayerFreeze();

        //Doorアニメーションを実行
        GameDirector.Instance().audioDirector.PlaySE("autoDoor");
        doorAnim.speed = 0.3f;
        doorAnim.Play("FacingDoor");
        doorAnim.gameObject.GetComponent<Collider>().enabled = false;

        float animTime = doorAnim.GetCurrentAnimatorStateInfo(0)
            .normalizedTime;
        if (animTime >= 1) {
            isEnded = true;
            Camera.main.GetComponent<CameraDirector>().
                RemoveObserver(GetComponent<AttentionCamera>());
            GameDirector.Instance().ReleasePlayerFreeze();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            can_start = true;
        }
    }
}
