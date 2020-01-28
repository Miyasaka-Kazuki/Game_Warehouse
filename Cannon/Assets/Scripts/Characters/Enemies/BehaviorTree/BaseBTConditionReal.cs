using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//実際に条件判定をするクラスのベースクラス
public abstract class BaseBTConditionReal : MonoBehaviour {
    public abstract BaseBTNode.NodeStatus EvaluateCondition();
    public virtual void Initialize(GameObject usingObj) { }
}
