using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//クリア画面遷移クラス
public class Clear : MonoBehaviour {
	//更新関数
	void Update () {
        if (Input.GetButtonDown("Tell"))
            Application.LoadLevel("title");
	}
}
