using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器砲台(弱)コマンド
public class HodainGrubCommand : BaseWeaponCommand {
    private GameObject bulletPrefab;
    private WeaponStatus weaponStatus;
    private PlayerStatus status;
    private float intervalTimer;
    private float thretholdTime;
    private bool preAttakcButton;
    private int bulletNum; //弾数
    private int bulletMaxNum = 15;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponentInParent<PlayerStatus>();
        weaponStatus = usingObj.GetComponent<WeaponStatus>();
        intervalTimer = 0;
        thretholdTime = weaponStatus.gunIntervalTime;
        preAttakcButton = false;
        bulletNum = bulletMaxNum;
    }

    public override void Enter() {
        bulletPrefab = weaponStatus.GetWeaponPrefab("HodainBall");
        GameDirector.Instance().ui_Director.InstanceUI("WeaponUI", gameObject);
    }
		
    public override void Activate() {
        intervalTimer += Time.fixedDeltaTime;
        if (intervalTimer > thretholdTime && status.GetWeaponAttackButton() &&

            !preAttakcButton && bulletNum > 0) {
            intervalTimer = 0;
            bulletNum--;

            GameObject bullet = Instantiate(bulletPrefab, weaponStatus.weaponInstancePosition.position,
                player.rotation);

            bullet.transform.LookAt(bullet.transform.position + player.forward);
        }

        preAttakcButton = status.GetWeaponAttackButton();
    }
		
    public override void Exit() {
        GameDirector.Instance().ui_Director.DestroyUI("WeaponUI");
    }
		
    public override int GetAmmo() {
        return bulletNum;
    }

    public override int GetMaxAmmo() {
        return bulletMaxNum;
    }
		
    public void AddBulletNum(int b) {
        bulletNum += b;
        if (bulletNum > bulletMaxNum)
            bulletNum = bulletMaxNum;
    }
}

//武器砲台(強)コマンド
public class HighHodainGrubCommand : BaseWeaponCommand {
    private GameObject bulletPrefab;
    private WeaponStatus weaponStatus;
    private PlayerStatus status;
    private float intervalTimer;
    private float thretholdTime;
    private bool preAttakcButton;
    private int bulletNum; //弾数
    private int bulletMaxNum = 5;

	//初期化関数
    public override void Initialize(GameObject usingObj) {
        status = usingObj.GetComponentInParent<PlayerStatus>();
        weaponStatus = usingObj.GetComponent<WeaponStatus>();
        intervalTimer = 0;
        thretholdTime = weaponStatus.gunIntervalTime;
        preAttakcButton = false;
        bulletNum = bulletMaxNum;
    }
		
    public override void Enter() {
        bulletPrefab = weaponStatus.GetWeaponPrefab("HighHodainBall");
        GameDirector.Instance().ui_Director.InstanceUI("WeaponUI", gameObject);
    }
		
    //更新関数
    public override void Activate() {
        intervalTimer += Time.fixedDeltaTime;
        if (intervalTimer > thretholdTime && status.GetWeaponAttackButton() &&
            !preAttakcButton && bulletNum > 0) {
            intervalTimer = 0;
            bulletNum--;

            GameObject bullet = Instantiate(bulletPrefab, weaponStatus.weaponInstancePosition.position,
                player.rotation);

            bullet.transform.LookAt(bullet.transform.position + player.forward);
        }
        preAttakcButton = status.GetWeaponAttackButton();
    }

    public override void Exit() {

        GameDirector.Instance().ui_Director.DestroyUI("WeaponUI");
    }


    public override int GetAmmo() {

        return bulletNum;
    }


    public override int GetMaxAmmo() {

        return bulletMaxNum;
    }


    public void AddBulletNum(int b) {

        bulletNum += b;
        if (bulletNum > bulletMaxNum)
            bulletNum = bulletMaxNum;
    }
}
