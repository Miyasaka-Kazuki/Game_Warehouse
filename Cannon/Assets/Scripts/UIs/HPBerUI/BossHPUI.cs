using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//ボスのHPバーUIクラス
public class BossHPUI : BaseUI {
    private BossStatus status;
    private float curHP;
    private Slider hpBer;

	//初期化関数
    public override void Initialize(GameObject callObj) {
        status = callObj.GetComponent<BossStatus>();
        hpBer = GetComponent<Slider>();
    }

	//更新関数
    public override void ActivateUI() {
        curHP = (float)status.GetHealth() / status.GetMaxHealth();
        hpBer.value = curHP;
    }
}
