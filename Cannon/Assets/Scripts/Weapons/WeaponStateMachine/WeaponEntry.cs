using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器状態遷移のベースクラス(それぞれの状態変数の初期化を含む)
public abstract class WeaponEntry : MonoBehaviour {
	protected static HodainGrubState hodainGrubState; //武器砲台(弱)
	protected static HighHodainGrubState highHodainGrubState; //武器砲台(強)

	//武器コマンド
    protected static HodainGrubCommand hodainGrubCommand;
    protected static HighHodainGrubCommand highHodainGrubCommand;
    private List<WeaponEntry> statusList; //武器切り替えに使うもの

	//初期化関数
    public void Initialize (GameObject usingObj) {
        BaseWeaponCommand[] command = new BaseWeaponCommand[2];
        GameObject weaponObj = usingObj.GetComponentInChildren<WeaponStatus>().gameObject;
        hodainGrubCommand = weaponObj.AddComponent<HodainGrubCommand>();
        highHodainGrubCommand = weaponObj.AddComponent<HighHodainGrubCommand>();
        command[0] = hodainGrubCommand;
        command[1] = highHodainGrubCommand;
        for (int i = 0; i < command.Length; i++) {
            command[i].Initialize(weaponObj);
        }

        WeaponEntry[] weapon = new WeaponEntry[2];
        hodainGrubState = weaponObj.AddComponent<HodainGrubState>();
        highHodainGrubState = weaponObj.AddComponent<HighHodainGrubState>();
        weapon[0] = hodainGrubState;
        weapon[1] = highHodainGrubState;
        for (int i = 0; i < weapon.Length; i++) {
            weapon[i].SubInitialize(weaponObj);
        }

        //サブ初期化
        weaponObj.GetComponentInChildren<WeaponStateMachine>().SubInitialize(weaponObj);

        //武器イメージとステートとの対応付け
        statusList = new List<WeaponEntry>();
        for (int i = 0; i < weapon.Length; i++) {
            statusList.Add(weapon[i]);
        }
    }

    protected virtual void SubInitialize(GameObject usingObj) { } //初期化関数
    public virtual void Enter() { } //武器切り替え直後に呼ばれる関数
    public abstract void Activate(); //実行関数
    public virtual void Exit() { } //別の武器切り替え直前に呼ばれる関数
    public virtual void IsChanging(WeaponStateMachine upperState) { } //武器切り替え関数

	//取得関数
    public virtual int GetAmmo() { return -1; } 
    public virtual int GetMaxAmmo() { return -1; }
    public virtual Texture GetImage() { return null; }
    public List<WeaponEntry> GetStatusList() { return statusList; }

	//弾数追加関数
	public virtual void AddBulletNum(int b) { }
}
