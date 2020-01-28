using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子オブジェクトから接触判定結果を受け取るクラスのベースクラス
public abstract class ParentTrigger : MonoBehaviour {
    public virtual void TriggerOnParent(Collider col) { }
}
