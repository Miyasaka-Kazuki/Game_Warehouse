using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//武器の弾数UIクラス
public class AmmoUI : BaseUI {
    private WeaponStatus status;
    private Text ammoText;
    [SerializeField] private RawImage image;

	//初期化関数
    public override void Initialize(GameObject callObj) {
        status = callObj.GetComponent<WeaponStatus>();
        ammoText = GetComponentInChildren<Text>();
    }

	//更新関数
    public override void ActivateUI() {
        image.texture = status.GetCurrentWeaponImage();

        int ammo = status.GetCurrentWeaponAmmo();
        string ammoStr = "";
        if (ammo < 10) ammoStr = "  ";
        ammoText.text = ammoStr + status.GetCurrentWeaponAmmo() + "/" + status.GetCurrentWeaponMaxAmmo();
    }

}
