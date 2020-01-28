using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器コマンドのベースクラス
public abstract class BaseWeaponCommand : MonoBehaviour {
    protected Transform player; //プレイヤー座標

	//初期化
    private void Start() {
        player = GetComponentInParent<PlayerDirector>().transform;
    }

    public virtual void Initialize(GameObject usingObj) { } //初期化
    public abstract void Activate(); //実行関数
    public virtual void Enter() { } //切り替え直後に呼ばれる関数
    public virtual void Exit() { } //別の武器の切り替え直前に呼ばれる関数

	//取得関数
    public virtual int GetAmmo() { return -1; } //
    public virtual int GetMaxAmmo() { return -1; }
}
