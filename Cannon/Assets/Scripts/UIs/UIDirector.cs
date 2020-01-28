using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//UI全体を管理するクラス
public class UIDirector : MonoBehaviour {
    public GameObject[] UI_prefabs;
    private SortedList<string, BaseUI> instanceMap;
    private SortedList<string, GameObject> tempInstanceMap;
    private SortedList<string, GameObject> tempCallObj;
    private Transform canvas;

	//初期化関数
	public void Start () {
        instanceMap = new SortedList<string, BaseUI>(3);
        tempInstanceMap = new SortedList<string, GameObject>();
        tempCallObj = new SortedList<string, GameObject>();
        canvas = GetComponentInChildren<Canvas>().transform;
	}

	//更新関数
	void Update () {
        //UIそれぞれを実行する
        for (int i = instanceMap.Count - 1; i >= 0; i--)
            instanceMap.Values[i].ActivateUI();
    }
		
	//UIの生成関数
    public void InstanceUI(string UI_name, GameObject callObj) {

        for (int i = 0; i < UI_prefabs.Length; i++) {
            if (!UI_prefabs[i].name.Equals(UI_name)) continue; //名前と一致していない
            if (instanceMap.ContainsKey(UI_name)) {
                //初期化処理のみ行う
                instanceMap[UI_name].GetAgainInInstance(callObj);
                return;
             }

            GameObject insUI = Instantiate(UI_prefabs[i], canvas);
            UIType type = insUI.GetComponent<BaseUI>().GetUIType();
            if (type != UIType.Canvas) insUI.transform.parent = null;

            instanceMap.Add(UI_name, insUI.GetComponent<BaseUI>()); //add
            insUI.GetComponent<BaseUI>().Initialize(callObj);

            tempCallObj.Add(UI_name, callObj);
            break;
        }
    }
		
	//UIの削除関数
    public void DestroyUI(string UI_name) {

        if (!instanceMap.ContainsKey(UI_name)) return;

        tempCallObj.Remove(UI_name);

        BaseUI will_desUI = instanceMap[UI_name]; //避難させておく
        instanceMap.Remove(UI_name);
        Destroy(will_desUI.gameObject);

    }


    //一時的に避難させる関数
    public void TempDestroy() {
        if (tempInstanceMap.Count != 0) return;

        for (int i = instanceMap.Count-1; i >= 0; i--) {
            tempInstanceMap.Add(instanceMap.Keys[i], tempCallObj.Values[i]);

            DestroyUI(instanceMap.Keys[i]);
        }
    }
		
    //避難したものを復活させる関数
    public void ReturnInstance() {
        if (tempInstanceMap.Count == 0) return;

        for (int i = tempInstanceMap.Count - 1; i >= 0; i--) {
            InstanceUI(tempInstanceMap.Keys[i], tempInstanceMap.Values[i]);
        }
        tempInstanceMap.Clear();
    }

	public bool HasInstance(string UI_name){
		return instanceMap.ContainsKey (UI_name);
	}
}
