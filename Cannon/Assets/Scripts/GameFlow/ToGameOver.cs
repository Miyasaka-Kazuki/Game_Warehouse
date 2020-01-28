using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームオーバー画面遷移クラス
public class ToGameOver : MonoBehaviour {

	//プレイヤー接触開始関数
    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            toGameOver();
        } else if (other.tag == "Enemy") {
            Destroy(other.gameObject);
        }
    }

	//ゲームオーバー遷移関数
    public void toGameOver() {
        //前のステージでボス戦に戻るか決める
        PlayerPrefs.SetString("PreScene", Application.loadedLevelName);
        Application.LoadLevel("Game over");
    }
}
