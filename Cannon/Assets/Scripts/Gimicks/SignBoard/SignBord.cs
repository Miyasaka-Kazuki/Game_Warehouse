using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//掲示板ギミッククラス
public class SignBord : MonoBehaviour {
    private Sentence sentence; //文章オブジェクト
    private bool isInSentence; //文章が存在するか
    private bool isInSignBoard; //掲示板が存在するか
    private float lastTime; //同じフレームで反応しないため

	//初期化関数
	void Start () {
        sentence = GetComponent<Sentence>();
        isInSentence = false;
        isInSignBoard = false;
        lastTime = 0;
    }
	
	//更新関数
	void Update () {
        if (Input.GetButtonDown("Tell") && !isInSentence && 
            isInSignBoard && lastTime != Time.time) {
            isInSentence = true;
            GameDirector.Instance().ui_Director.DestroyUI("BalloonIcon");
            GameDirector.Instance().ui_Director.InstanceUI("ConversationUI", gameObject);
            GameDirector.Instance().AllFreeze(gameObject);
        }
    }

	//プレイヤーが近づいた瞬間に呼ばれる関数
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameDirector.Instance().ui_Director.InstanceUI("BalloonIcon", gameObject);
            isInSignBoard = true;
        }
    }

	//プレイヤーが離れた瞬間に呼ばれる関数
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameDirector.Instance().ui_Director.DestroyUI("BalloonIcon");
            isInSignBoard = false;
        }
    }

	//文章終了関数
    public void FinishConversation() {
        lastTime = Time.time;
        isInSentence = false;
        GameDirector.Instance().ui_Director.InstanceUI("BalloonIcon", gameObject);
        GameDirector.Instance().ui_Director.DestroyUI("ConversationUI");
        GameDirector.Instance().ReleaseAllFreeze(gameObject);
    }

	//文章取得関数
    public string[] GetSentence() {
        return sentence.GetSentence();
    }

	//文章が存在するか
    public bool GetIsInSentence() {
        return isInSentence;
    }
}
