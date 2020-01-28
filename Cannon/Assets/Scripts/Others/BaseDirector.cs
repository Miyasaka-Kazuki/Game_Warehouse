using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理する対象のタイプ
public enum DirectorType {
    Character,
    Camera,
    Other,
}
//管理オブジェクトの抽象クラス
public abstract class BaseDirector : MonoBehaviour {
    protected List<BaseObserver> baseObserver;
    protected DirectorType type;

	//管理するオブジェクトの追加
    public void AddObserver(BaseObserver oo) {
        if (oo == null) return;

        if (baseObserver.Contains(oo)) return;
        baseObserver.Add(oo);
        oo.Enter(gameObject);
    }


	//管理するオブジェクトの削除
    public void RemoveObserver(BaseObserver oo) {
        if (oo == null) return;

        if (!baseObserver.Contains(oo)) return;
        baseObserver.Remove(oo);
        oo.Exit(gameObject);
    }

    public DirectorType GetDirectorType() {
        return type;
    }
}
