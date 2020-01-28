using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//壁の食らい判定表現クラス
public class WallHit : MonoBehaviour {
    Renderer rend;
    bool isHit;
    float timer;

	//初期化処理
	void Start () {
        rend = GetComponent<Renderer>();
        isHit = false;
        timer = 0;
	}
	
	//更新処理
	void Update () {
		if (isHit) {
            Color color = rend.material.color;
            color.r /= 2;
            color.g /= 2;
            color.b /= 2;

			rend.material.color = color;
            timer += Time.deltaTime;
            if (timer > 0.1f) {
                rend.material.color = Color.white;
                timer = 0;
                isHit = false;
            }
        }
	}

	//食らい表現開始に呼ばれる関数
    public void OnTriggerEnter(Collider other) {
        isHit = true;
    }
}
