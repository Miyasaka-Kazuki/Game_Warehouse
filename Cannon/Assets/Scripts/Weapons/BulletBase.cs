using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器のベースクラス
public abstract class BulletBase : OtherMoveMachine {
    protected int bulletValue; //武器のダメージ量

	//武器ダメージの取得関数
    public int GetWeaponValue() {
        return bulletValue;
    }

}
