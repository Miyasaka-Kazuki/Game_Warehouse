using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//メイン画面遷移クラス
public class  ToMain : MonoBehaviour {
    public Text start_text;
    public GameObject omati;
    public AudioSource source;
    public AudioClip clip;

	//更新関数
	void Update () {
        if (Input.GetButtonDown("ToMain")) {
            source.PlayOneShot(clip);
            start_text.color = Color.black;
            omati.SetActive(true);
            Application.LoadLevel("MainStage");
        }
    }
}
			 