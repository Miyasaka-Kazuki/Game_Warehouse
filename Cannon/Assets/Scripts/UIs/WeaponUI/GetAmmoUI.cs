using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAmmoUI : BaseUI {
    InventoryItem types;
    Text text;
    float timer;
    float threshold = 3;

    public override void Initialize(GameObject callObj) {
        types = callObj.GetComponent<item>().type;
        text = GetComponentInChildren<Text>();
        timer = 0;
    }

    public override void ActivateUI() {
        switch (types) {
            case InventoryItem.HEALTH_UP_ITEM:
                text.text = "回復薬";
                break;
            case InventoryItem.WEAPON_GUN:
                text.text = "弾薬パック：銃";
                break;
            case InventoryItem.WEAPON_HODAIN:
                text.text = "弾薬パック：青の砲台";
                break;
            case InventoryItem.WEAPON_HIGHHODAIN:
                text.text = "弾薬パック：赤の砲台";
                break;
        }

        timer += Time.deltaTime;
        if (timer > threshold) {
            GameDirector.Instance().ui_Director.DestroyUI("GetBulletAmmoUI");
        }
    }

    public override void GetAgainInInstance(GameObject callObj) {
        types = callObj.GetComponent<item>().type;
        timer = 0;
    }
}
