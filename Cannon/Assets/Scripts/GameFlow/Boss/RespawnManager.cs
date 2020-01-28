using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//アイテム無限生成クラス
public class RespawnManager : MonoBehaviour {
    public GameObject spawn_prefab;
    public GameObject first_item; //最初の既に出現しているアイテム
    public float respawnThresholdTime = 30f;
    private float respawnTimer;
    private Vector3 spawn_position;
    private GameObject now_spawn_item;

	//初期化関数
	void Start () {
        respawnTimer = 0;
        spawn_position = first_item.transform.position;
        now_spawn_item = first_item;
	}
	
	//更新関数
	void Update () {
        //前のアイテムがとられるまで数えない
        if (now_spawn_item != null) return;

        respawnTimer += Time.deltaTime;
		if (respawnTimer > respawnThresholdTime) {
            respawnTimer = 0;
            now_spawn_item = Instantiate(spawn_prefab, spawn_position, Quaternion.identity);
        }
	}
}
