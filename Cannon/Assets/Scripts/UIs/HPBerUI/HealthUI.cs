using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//プレイヤーのHPUIクラス
public class HealthUI : BaseUI {
    private PlayerStatus status;
    private RawImage[] harts;
    private float timer;
    private float threshold = 3;

	//初期化関数
    public override void Initialize(GameObject callObj) {
        status = callObj.GetComponent<PlayerStatus>();
        harts = GetComponentsInChildren<RawImage>();
    }

	//更新関数
    public override void ActivateUI() {
        int curNum = status.GetHealth();
        int maxNum = status.GetMaxHealth();

        for (int i = 0; i < maxNum; i++) {
            if (i < curNum)
                harts[i].gameObject.SetActive(true);
            else
                harts[i].gameObject.SetActive(false);
        }

        timer += Time.deltaTime;
        if (timer > threshold) {
            GameDirector.Instance().ui_Director.DestroyUI("HP");
        }
    }

    public override void GetAgainInInstance(GameObject callObj) {
        timer = 0;
    }
}
