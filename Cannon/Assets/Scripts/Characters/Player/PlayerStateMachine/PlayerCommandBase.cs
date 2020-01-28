using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤー行動コマンドのベースクラス
public abstract class PlayerCommandBase : MonoBehaviour {
    public virtual void Initialize(GameObject usingObj) { } //コマンド初期化関数
    public virtual void Enter() {} //コマンドを開始した瞬間関数
    public abstract Vector3 Activate(ref Vector3 lookAtPos); //コマンド処理関数
    public virtual void Exit() {} //コマンドを終了した瞬間関数
}
