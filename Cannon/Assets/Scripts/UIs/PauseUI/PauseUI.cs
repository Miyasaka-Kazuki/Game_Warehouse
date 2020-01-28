using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ポーズUIクラス
public class PauseUI : BaseUI {
	private bool init;

	//初期化関数
    public override void Initialize(GameObject callObj) {
		init = true;
    }

	//更新関数
    public override void ActivateUI() {
		if (init) {
			init = false;
			return;
		}

		if (Input.GetButtonDown ("ToTitle")) {
			Application.LoadLevel ("title");
		} else if (Input.GetButtonDown ("ToBack") || Input.GetButtonDown ("Pause")) {
            GameDirector.Instance().ui_Director.DestroyUI("PauseUI");
		}
    }
}
