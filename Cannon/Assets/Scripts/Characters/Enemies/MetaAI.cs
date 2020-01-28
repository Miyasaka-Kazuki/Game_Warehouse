using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エネミーグループの管理クラス
public class MetaAI : MonoBehaviour {
    public GameObject[] enemyGroup_prefabs; //プレハブ
    private Dictionary<GameObject, Transform> instanceMap; //<callObj, enemyGroup>
    private Transform belgianAI;

	//初期化関数
    void Start () {
        instanceMap = new Dictionary<GameObject, Transform>(2);
        belgianAI = GetComponentInChildren<BelgianAI>().transform;
	}
		
	//エネミグループをプレハブからインスタンスする関数
    public void InstanceEnemy(string enemyGroup_name, GameObject callObj, GameObject parentObj = null) {

        for (int i = 0; i < enemyGroup_prefabs.Length; i++) {

            //プレハブにないなら生成しない
            if (!enemyGroup_prefabs[i].name.Equals(enemyGroup_name)) continue; //名前と一致していない

            //生成したり登録したり
            GameObject enemyGroup = Instantiate(enemyGroup_prefabs[i], belgianAI);
            if (parentObj != null) enemyGroup.transform.parent = parentObj.transform;
            enemyGroup.transform.position = callObj.transform.position;
            instanceMap.Add(callObj, enemyGroup.transform); //add

            //rootがEnemyDirectorなら
            EnemyDirector single = enemyGroup.GetComponent<EnemyDirector>();
            if (single != null) {
                GameDirector.Instance().AddEnemyDirector(single);
                return;
            }

            //rootの子オブジェクトがEnemyDirectorなら
            EnemyDirector[] group = enemyGroup.GetComponentsInChildren<EnemyDirector>();
            for (int num = 0; num < group.Length; num++)
                GameDirector.Instance().AddEnemyDirector(group[num]);
            return;
        }
    }
		
	//インスタンスからエネミーを削除する関数
    public void DestroyEnemy(GameObject callObj) {
        //生成されていないなら
        if (!instanceMap.ContainsKey(callObj)) return;

        //削除
        Transform will_desEneGroup = instanceMap[callObj]; //避難させておく
        instanceMap.Remove(callObj);

        //rootがEnemyDirectorなら
        if (will_desEneGroup == null)
            return;
        EnemyDirector single = will_desEneGroup.GetComponent<EnemyDirector>();
        if (single != null) {
            GameDirector.Instance().RemoveEnemyDirector(single);
            Destroy(will_desEneGroup.gameObject);
            return;
        }

        //rootの子オブジェクトがEnemyDirectorなら
        EnemyDirector[] group = will_desEneGroup.GetComponentsInChildren<EnemyDirector>();
        for (int num = 0; num < group.Length; num++)
            GameDirector.Instance().RemoveEnemyDirector(group[num]);
        Destroy(will_desEneGroup.gameObject);
        return;
    }
}
