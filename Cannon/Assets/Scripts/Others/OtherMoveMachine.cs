using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの行動状態遷移クラスのベースクラス
public abstract class OtherMoveMachine : MonoBehaviour {
    public abstract Vector3 Activate();
}
