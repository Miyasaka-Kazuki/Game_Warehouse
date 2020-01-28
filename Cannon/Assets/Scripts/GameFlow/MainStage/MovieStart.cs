using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

//全て停止させるか対象のみ停止させるか
public enum FreezeObj {
    all,
    applyOnly,
}

//ムービー開始関数
public class MovieStart : MonoBehaviour {

    //applyObjごとにそれぞれ入れる
    [SerializeField] GameObject applyObj;
    [SerializeField] string parentAnim_name;
    [SerializeField] string[] childAnim_name;
    [SerializeField] float[] childAnim_second;
    [SerializeField] FreezeObj freezeObj;
    [SerializeField] bool can_skip = false;

    private List<string> childAnim_nameList;
    private List<float> childAnim_secondList;
    private bool movieEnded;

	//初期化関数
    void Start () {

        if (childAnim_name.Length != childAnim_second.Length)
            Debug.Log(gameObject + " : animの名前と秒の数が一致してないよ！");

        //初期化処理
        childAnim_nameList = new List<string>();
        childAnim_secondList = new List<float>();
        for (int i = 0; i < childAnim_name.Length; i++) {
            childAnim_nameList.Add(childAnim_name[i]);
            childAnim_secondList.Add(childAnim_second[i]);
        }
        if (childAnim_name.Length == 0) {
            childAnim_nameList = null;
            childAnim_secondList = null;
        }

        movieEnded = false;
	}

	//ムービー開始点接触関数
    private void OnTriggerEnter(Collider other) {

        //ムービースタート
        if (other.tag == "Player" && !movieEnded) {

            movieEnded = true;
            GameDirector.Instance().ApplyAnim(
                applyObj, parentAnim_name, childAnim_nameList, childAnim_secondList, 
                freezeObj, can_skip);
        }
    }
}
