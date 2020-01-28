using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//文章表示UIクラス
public class ConversationUI : BaseUI {
    private SignBord signBord;
    private String[] sentence;
    private Text text;
    private int subScript;
    private float firstTime; //最初のフレームでEnterに反応しないため

	//初期化関数
    public override void Initialize(GameObject callObj) {
        signBord = callObj.GetComponent<SignBord>();
        sentence = signBord.GetSentence();
        text = GetComponentInChildren<Text>();
        subScript = 0;

        // \\nがあったら\nに修正
        for (int i = 0; i < sentence.Length; i++) {
            sentence[i] = sentence[i].Replace("\\n", "\n");
        }
        firstTime = Time.time;
    }

	//更新関数
    public override void ActivateUI() {
        text.text = sentence[subScript];

        if (Input.GetButtonDown("Tell") && firstTime != Time.time) {
            if (subScript == sentence.Length - 1)
                signBord.FinishConversation();
            else
                subScript++;
        }
    }
}
