using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器砲台の弾クラス
public class HodainBall : OtherMoveMachine {
    public GameObject instancePrefab; //プレハブ
	public float lifeTime = 5;
    private float speed; 
    private float gravity;
    private float preSpeed;
	private float destroyTimer; //削除タイマー

	//初期化関数
	void Start () {
        speed = 7;
        gravity = 9.8f;
		preSpeed = 3;
		destroyTimer = 0;
	}

	//更新関数
    public override Vector3 Activate() {
        Vector3 movePos = Vector3.zero;
        movePos = transform.forward * speed * Time.fixedDeltaTime;

        float nowSpeed = preSpeed - gravity * Time.fixedDeltaTime;
        movePos += nowSpeed * Vector3.up * Time.fixedDeltaTime;

        preSpeed = nowSpeed;

        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore);

        float magnitude = transform.position.y - hit.point.y;

        if (hit.transform != null) {
            if (magnitude <= 0.3f && hit.transform.tag == "Untagged") {
                GameObject hodainObj = Instantiate(instancePrefab, transform.position + Vector3.up * 0.5f, transform.rotation);
                hodainObj.GetComponent<HodainGrub>().Initialize();
                Destroy(this.gameObject);
            }
        }

		destroyTimer += Time.fixedDeltaTime;
		if (destroyTimer > lifeTime) {
			Destroy(this.gameObject);
		}

        return movePos;
    }
}
