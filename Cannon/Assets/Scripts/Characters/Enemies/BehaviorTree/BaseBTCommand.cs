using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//行動コマンドのベースクラス
public abstract class BaseBTCommand : MonoBehaviour {
	public virtual void Initialize(GameObject usingObj) { } //初期化処理
	public virtual void Initialize(float timeOrCount) { } //初期化処理

	public virtual void Enter() { } //行動する瞬間に呼び出す関数
    public abstract BaseBTNode.NodeStatus Activate(); //実行処理関数
    public virtual void Exit() { } //行動が終了する瞬間に呼び出す関数
    public virtual void ExitForSetFailuer() { } //ノードの状態がFailureになった時終了する瞬間に呼び出す関数
}
