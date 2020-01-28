using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボス画面遷移クラス
public class ToBoss : MonoBehaviour {
    public void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            PlayerPrefs.SetInt("HP", status.GetHealth());
            Application.LoadLevel("FirstBoss");
        }
    }
}
