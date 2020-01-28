using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

//ムービー全体を管理するクラス
public class MovieDirector : MonoBehaviour {
    [SerializeField] private GameObject parentPrefab; //applyObjの位置を移動するオブジェクト
    private List<GameObject> curRegisList; //現在登録されアニメーションされているオブジェクトリスト
    private List<OrderedDictionary> mapList; //モーションと再生時間のマップリスト
    private List<int> subScript;
    private List<float> timer;

	//初期化関数
    public void Start() {
        curRegisList = new List<GameObject>();
        mapList = new List<OrderedDictionary>();
        subScript = new List<int>();
        timer = new List<float>();
        transform.position = Vector3.zero;
    }

	//更新関数
    void Update() {
        //ani_sec_mapListがあるなら最後の添え字まで実行して削除
        //timeのリストとsubScriptのリストをつくる
        for (int i = mapList.Count - 1; i >= 0; i--) {
            if (mapList[i] == null) continue;

            curRegisList[i].transform.localPosition = Vector3.zero;

            timer[i] += Time.deltaTime;
            OrderedDictionary dic = mapList[i];
            //指定された再生時間にまだ達してないなら
            if (timer[i] < (float)dic[subScript[i]]) continue;

            //ネクスト処理
            subScript[i]++;
            timer[i] = 0;

            //最後まで再生したものは全てから削除する
            if (subScript[i] > mapList.Count - 1) {
                curRegisList.RemoveAt(i);
                mapList.RemoveAt(i);
                subScript.RemoveAt(i);
                timer.RemoveAt(i);

                //キーの指定は一旦配列に取り出してから検索する
                string[] childAnimKey = new string[dic.Count];
                dic.Keys.CopyTo(childAnimKey, 0);
                curRegisList[i].GetComponent<Animator>().Play(childAnimKey[subScript[i]]);
            }
        }
    }


    //applyObj  = アニメーションを適用するオブジェクト
    //parentAnim = applyObjの親オブジェクトが実行するアニメーション
    //ani_sec_map<string, float> = 独自に動く場合、applyObjが前から順番に実行するアニメーションと再生時間
	//アニメーションを適用させるオブジェクト追加関数
    public void ApplyAnim(GameObject applyObj, string parentAnim_name, OrderedDictionary ani_sec_map = null) {

        //既に再生中かどうか調べる
        if (curRegisList.Contains(applyObj)) return;
        curRegisList.Add(applyObj);
        mapList.Add(ani_sec_map);
        subScript.Add(0);
        timer.Add(0);
        //subScript[curRegisList.Count-1] = 0;
        //timer[curRegisList.Count - 1] = 0;

        //空オブジェクトを生成する
        GameObject parentObj = Instantiate(parentPrefab);
        parentObj.transform.parent = transform;

        //applyObjを子オブジェクトにする
        applyObj.transform.parent = parentObj.transform;
        //        applyObj.transform.localPosition = Vector3.zero;

        //空オブジェクトはparentAnimを実行する
        //"Nothing"を受け取った場合は何もしない
        Animator parentAnim = parentObj.GetComponent<Animator>();
        if (parentAnim_name.Equals("Nothing")) {
            parentAnim.applyRootMotion = true;
        } else {
            parentAnim.Play(parentAnim_name);
        }

        //applyObjはani_sec_map.Keys[0]を実行する
        //nullならHumanoidのアニメーションをしない
        if (ani_sec_map != null) {
            Animator childAnim = applyObj.GetComponent<Animator>();
            string[] childAnimKey = new string[ani_sec_map.Count];
            ani_sec_map.Keys.CopyTo(childAnimKey, 0);
            childAnim.Play(childAnimKey[0]);
        }
    }
}
