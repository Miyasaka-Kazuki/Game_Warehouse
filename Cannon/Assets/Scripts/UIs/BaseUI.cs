using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType {
    Canvas,
    Other,
}
//UIクラスのベースクラス
public abstract class BaseUI : MonoBehaviour {
    [SerializeField] private UIType type; //UIタイプ
    public abstract void Initialize(GameObject callObj); //初期化関数
    public abstract void ActivateUI(); //更新関数

	//UIタイプ取得関数
    public UIType GetUIType() {
        return type;
    }

	//連続でUIインスタンスが生成されたとき呼ばれる関数
    public virtual void GetAgainInInstance(GameObject callObj) { }
}
