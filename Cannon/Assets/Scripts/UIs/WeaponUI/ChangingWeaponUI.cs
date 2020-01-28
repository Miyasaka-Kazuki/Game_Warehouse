using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//武器切り替えUIクラス
public class ChangingWeaponUI : BaseUI {
    public RawImage[] weapons;
    public Image[] frames;
    private int curImageNum;
    private WeaponStateMachine stateMachine;
    private List<WeaponEntry> statusList;
    private float preHor;

    public override void Initialize(GameObject callObj) {
        stateMachine = callObj.GetComponent<WeaponStateMachine>();
        statusList = stateMachine.GetStatusList();
        curImageNum = stateMachine.GetCurrentStateNum();

        //枠を取得
        frames = new Image[weapons.Length];
        for (int i = 0; i < weapons.Length; i++) {
            frames[i] = weapons[i].GetComponentInChildren<Image>();
        }
    }

    public override void ActivateUI() {
        float hor = Input.GetAxis("ChangeWeaponChoice");
        if (preHor == 0 && hor < -0.5f) {
            if (curImageNum > 0)
            curImageNum--;
        } else if (preHor == 0 && hor > 0.5) {
            if (curImageNum < weapons.Length-1)
                curImageNum++;
        }
        preHor = hor;

        //今の場所を設定
        stateMachine.SetCurrentSwitchNum(curImageNum);
    }
}
