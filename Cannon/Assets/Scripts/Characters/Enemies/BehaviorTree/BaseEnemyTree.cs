using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ビヘイビアツリー(行動管理)のベースクラス
public abstract class BaseEnemyTree : MonoBehaviour {
    public virtual Vector3 Activate(ref Vector3 lookAtPos) { return Vector3.zero; } //更新関数
    public virtual Vector3[] ActivateBoss(Vector3[] lookAtPos) { return null; } //ボスの更新関数
    public virtual void SetMovePosition(Vector3 pos) { } //エネミーの移動ポジション
	public virtual void SetMovePosition(Vector3[] pos) { } //エネミーの移動ポジション
	public virtual void SetLookAtPos(Vector3 pos) { } //エネミーのアングル
    public virtual void SetLookAtPos(Vector3[] pos) { } //エネミーのアングル
}