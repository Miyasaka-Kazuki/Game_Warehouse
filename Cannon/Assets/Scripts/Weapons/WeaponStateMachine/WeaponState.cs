using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器砲台(弱)クラス
public class HodainGrubState : WeaponEntry {
    private Texture image;

	//初期化関数
    protected override void SubInitialize(GameObject usingObj) {
        image = Resources.Load("WeakHodainGrub") as Texture;
    }

    public override void Enter() {
        hodainGrubCommand.Enter();
    }

    public override void Activate() {
        hodainGrubCommand.Activate();
    }

    public override void Exit() {
        hodainGrubCommand.Exit();
    }
		
    public override void IsChanging(WeaponStateMachine upperState) {
    }

    public override int GetAmmo() {
        return hodainGrubCommand.GetAmmo();
    }
    public override int GetMaxAmmo() {
        return hodainGrubCommand.GetMaxAmmo();
    }

    public override Texture GetImage() {
        return image;
    }
    public override void AddBulletNum(int b) {
        hodainGrubCommand.AddBulletNum(b);
    }
}

//武器砲台(強)クラス
public class HighHodainGrubState : WeaponEntry {
    private Texture image;

	//初期化関数
    protected override void SubInitialize(GameObject usingObj) {
        image = Resources.Load("HighHodainGrub") as Texture;
    }

    public override void Enter() {
        highHodainGrubCommand.Enter();
    }

    public override void Activate() {
        highHodainGrubCommand.Activate();
    }

    public override void Exit() {
        highHodainGrubCommand.Exit();
    }

    public override void IsChanging(WeaponStateMachine upperState) {
    }

    public override int GetAmmo() {
        return highHodainGrubCommand.GetAmmo();
    }
    public override int GetMaxAmmo() {
        return highHodainGrubCommand.GetMaxAmmo();
    }

    public override Texture GetImage() {
        return image;
    }
    public override void AddBulletNum(int b) {
        highHodainGrubCommand.AddBulletNum(b);
    }
}

