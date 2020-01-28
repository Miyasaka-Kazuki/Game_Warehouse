using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//条件判定をするコンディションノードクラスのベースクラス
public abstract class BaseBTCondition : BaseBTNode {
    public override NodeStatus Evaluate() {
        throw new NotImplementedException();
    }
	//条件判定関数
    public abstract NodeStatus EvaluateCondition(NodeStatus upperStatus);
}
