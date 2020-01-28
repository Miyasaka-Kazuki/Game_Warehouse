using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器の状態管理クラス
public class WeaponStatus : MonoBehaviour {
    public GameObject enemyBulletPrefab; //弾のプレハブ
    public Transform weaponInstancePosition; //武器生成座標
    public float gunIntervalTime; //撃った後のインターバルタイム
    [SerializeField] private GameObject[] weaponPrefab; //武器プレハブ
    [SerializeField] private Material[] weaponImage; //武器画像
    private WeaponStateMachine stateMachine; //武器の状態遷移管理変数

    //初期化関数
    void Start () {
        stateMachine = GetComponent<WeaponStateMachine>();
	}
		

	//武器プレハブ取得関数
    public GameObject GetWeaponPrefab(string W_name) {
        for (int i = 0; i < weaponPrefab.Length; i++) {
            if (!weaponPrefab[i].name.Equals(W_name)) continue; //名前と一致していない
            return weaponPrefab[i];
        }
        return null;
    }

	//現在の武器画像取得関数
    public Texture GetCurrentWeaponImage() {
        return stateMachine.GetCurrentWeaponImage();
    }

	//現在の武器弾数取得関数
    public int GetCurrentWeaponAmmo() {
        return stateMachine.GetCurrentAmmo();
    }

	//現在の武器の最大弾数取得関数
    public int GetCurrentWeaponMaxAmmo() {
        return stateMachine.GetCurrentMaxAmmo();
    }

    //武器の弾数追加関数
    public void AddBulletNum(int b, int number) {
        stateMachine.AddBulletNum(b, number);
    }
}
