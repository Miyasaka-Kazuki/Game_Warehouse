using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器の切り替え状態管理クラス
public class WeaponStateMachine : WeaponEntry {
    private WeaponEntry currentWeapon; //現在の武器
    private int nowSwitchNum; //現在の武器番号
    private bool preSwitch; //前フレームに武器を切り替えたか

	//初期化関数
    protected override void SubInitialize (GameObject usingObj) {
        currentWeapon = hodainGrubState;
        currentWeapon.Enter();
        preSwitch = false;
	}
		
	//更新関数
	public override void Activate () {
        currentWeapon.Activate();
        currentWeapon.IsChanging(this);
	}
		
	//武器切り替え実行関数
    public void ChangeState(WeaponEntry nextState) {
        currentWeapon.Exit();
        currentWeapon = nextState;
        currentWeapon.Enter();
    }

    //武器切り替え関数
    public void SwitchWeapon() {
        if (Input.GetButtonUp("ChangeWeaponTime")) {
            GameDirector.Instance().ReleaseAllFreeze(gameObject);
            GameDirector.Instance().ui_Director.DestroyUI("ChangeWeaponUI");
        } else if (Input.GetButtonDown("ChangeWeaponTime")) {
            GameDirector.Instance().AllFreeze(gameObject);
            GameDirector.Instance().ui_Director.InstanceUI("ChangeWeaponUI", gameObject);
        } else  if (Input.GetButton("ChangeWeaponTime")){

            //違うなら切り替え
            if (currentWeapon != GetStatusList()[nowSwitchNum])
                ChangeState(GetStatusList()[nowSwitchNum]);
        }
    }

	//現在の弾数取得関数
    public int GetCurrentAmmo() {
        return currentWeapon.GetAmmo();
    }

	//現在の最大弾数関数
    public int GetCurrentMaxAmmo() {

        return currentWeapon.GetMaxAmmo();
    }

	//現在の武器画像関数
    public Texture GetCurrentWeaponImage() {
        return currentWeapon.GetImage();
    }

	//現在の武器インデックス取得関数
    public int GetCurrentStateNum() {
        return GetStatusList().IndexOf(currentWeapon);
    }

	//現在の武器番号取得関数
    public void SetCurrentSwitchNum(int n) {
        nowSwitchNum = n;
    }

	//武器の弾数追加
    public void AddBulletNum(int b, int number) {

        GetStatusList()[number].AddBulletNum(b);
    }
}
