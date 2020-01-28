using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ゲームオーバー画面遷移クラス
public class GameOver : MonoBehaviour {
    public GameObject toBossText;
    public GameObject toTitleText;

    public AudioClip clip;
    public AudioSource source;
    public Text text1;
    public Text text2;
    bool isToBoss;

	//初期化関数
    void Start () {
        isToBoss = true;
        toBossText.SetActive(true);
        if (PlayerPrefs.HasKey("PreScene")) {
            if (PlayerPrefs.GetString("PreScene") == "FirstBoss") {
                isToBoss = true;
                toBossText.SetActive(true);
            } else {
                isToBoss = false;
                toBossText.SetActive(false);
            }
            PlayerPrefs.DeleteKey("PreScene");
        }
    }

	//更新関数
	void Update () {
		if (Input.GetButtonDown("ToTitle")){
            text1.color = Color.black;
            source.PlayOneShot(clip);
            Application.LoadLevel("title");
        } else if (Input.GetButtonDown("ToBoss")) {
            if (isToBoss) {
                text2.color = Color.black;
                source.PlayOneShot(clip);
                Application.LoadLevel("FirstBoss");
            }
        }
    }
}
