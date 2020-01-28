using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ムービー時のUIクラス
public class MovieUI : BaseUI {
    private Animator anim;

    public override void Initialize(GameObject callObj) {
        anim = GetComponent<Animator>();
    }
		
    public override void ActivateUI() {
    }
}
