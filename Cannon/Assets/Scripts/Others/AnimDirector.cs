using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;

//アニメーション全体を管理するクラス
public class AnimDirector : MonoBehaviour {
    //アニメーションするオブジェクトの親オブジェクトと子オブジェクトの位置に影響される
    [SerializeField] private GameObject parentPrefab; //applyObjの位置を移動するオブジェクト
    private List<GameObject> curRegisList; //現在登録されアニメーションされているオブジェクトリスト
    private List<List<string>> animNameList;
    private List<List<float>> secondList;
    private List<int> origin_animClip;
    private List<float> origin_animTime;
    private List<int> subScript;
    private List<float> timer;
    private List<Transform> originParent;
    private List<Animator> returnAnimObj; //モーションを全部終了したときに元に戻すため

    private bool canSkip;

	//初期化関数
    public void Start() {
        curRegisList = new List<GameObject>();
        animNameList = new List<List<string>>();
        secondList = new List<List<float>>();
        origin_animClip = new List<int>();
        origin_animTime = new List<float>();
        subScript = new List<int>();
        timer = new List<float>();
        transform.position = Vector3.zero;
        originParent = new List<Transform>();
        returnAnimObj = new List<Animator>();

        canSkip = false;
    }

	//更新関数
    public void Activate() {
        for (int i = curRegisList.Count - 1; i >= 0; i--) {

            //animNameList[i] == nullということは
            //親オブジェクトしかアニメーションしていないということ
            if (animNameList[i] == null) {

                Animator anim = curRegisList[i].transform.parent.GetComponent<Animator>();
                float curTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (curTime >= 1) FinishAnyAnimPrefab(i);
                continue;
            }

            //ani_sec_mapでの諸々の処理
            timer[i] += Time.deltaTime;

            //指定された再生時間にまだ達してないなら終わり
            if (timer[i] < secondList[i][subScript[i]]) continue;

            //ネクスト処理
            subScript[i]++;
            timer[i] = 0;

            //ani_sec_mapのアニメーションの数が最後まで達したら
            if (subScript[i] > animNameList[i].Count - 1) {
                FinishAnyAnimPrefab(i);
                continue;
            }

            //次のアニメーションをプレイ
            //キーの指定は一旦配列に取り出してから検索する
            curRegisList[i].GetComponent<Animator>().Play(animNameList[i][subScript[i]]);
        }

        //終了処理
        bool movieSkip = canSkip && Input.GetButtonDown("MovieSkip");
        if (curRegisList.Count == 0 || movieSkip) {

            //スキップで強制終了したときのために
            for (int i = curRegisList.Count-1; i >= 0 ; i--) {
                FinishAnyAnimPrefab(i);
            }

            for (int i = 0; i < returnAnimObj.Count; i++) {
                returnAnimObj[i].Play(origin_animClip[i], -1, origin_animTime[i]);
                returnAnimObj[i].SetBool("isMovied", false);
            }

            curRegisList.Clear();
            animNameList.Clear();
            secondList.Clear();
            subScript.Clear();
            timer.Clear();
            originParent.Clear();
            origin_animClip.Clear();
            origin_animTime.Clear();
            returnAnimObj.Clear();

            GameDirector.Instance().FinishAnim();
        }
    }

    //applyObj  = アニメーションを適用するオブジェクト
    //parentAnim = applyObjの親オブジェクトが実行するアニメーション
    //ani_sec_map<string, float> = 独自に動く場合、applyObjが前から順番に実行するアニメーションと再生時間
    public void ApplyAnim(GameObject applyObj, string parentAnim_name, List<string> childAnim_name, List<float> childAnim_second, bool canSkip_ = false) {
        if (canSkip_)   canSkip = canSkip_;

        //既に再生中かどうか調べる
        if (curRegisList.Contains(applyObj)) return;
        curRegisList.Add(applyObj);
        animNameList.Add(childAnim_name);
        secondList.Add(childAnim_second);
        subScript.Add(0);
        timer.Add(0);

        //空オブジェクトを生成する
        GameObject parentObj = Instantiate(parentPrefab);
        parentObj.transform.parent = transform;

        //applyObjを子オブジェクトにする
        originParent.Add(applyObj.transform.parent);
        applyObj.transform.parent = parentObj.transform;

        //空オブジェクトはparentAnimを実行する
        //"Nothing"を受け取った場合は何もしない
            Animator parentAnim = parentObj.GetComponent<Animator>();
        if (parentAnim_name.Equals("Nothing")) {
            parentAnim.enabled = false;
        } else {
            parentAnim.Play(parentAnim_name);

            //parentAnimをやるならその場所で動く
            applyObj.transform.localPosition = Vector3.zero;
            applyObj.transform.localRotation = Quaternion.identity;
        }

        //applyObjはani_sec_map.Keys[0]を実行する
        //nullならHumanoidのアニメーションをしない
        if (childAnim_name != null) {
            Animator childAnim = applyObj.GetComponent<Animator>();
            childAnim.SetBool("isMovied", true);

            //やるまえのアニメーションを保存しておく
            returnAnimObj.Add(childAnim);
            origin_animClip.Add(childAnim.GetCurrentAnimatorStateInfo(0).fullPathHash);
            origin_animTime.Add(childAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
            childAnim.Play(childAnim_name[0]);
        }
    }


    // i : リストに登録されている場所
	//アニメーション終了関数
    private void FinishAnyAnimPrefab(int i) {
        //元に戻す
        Transform parentAnimObj = curRegisList[i].transform.parent;
        curRegisList[i].transform.parent = originParent[i];
        Destroy(parentAnimObj.gameObject);

        curRegisList.RemoveAt(i);
        animNameList.RemoveAt(i);
        secondList.RemoveAt(i);
        subScript.RemoveAt(i);
        timer.RemoveAt(i);
        originParent.RemoveAt(i);
    }
}
