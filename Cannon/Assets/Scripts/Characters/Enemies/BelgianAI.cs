using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//近づく敵がプレイヤーになだれ込まないようにする制限管理クラス
public class BelgianAI : MonoBehaviour {
    public Transform player; //プレイヤーの位置

    private static List<GameObject> findEnemies; //プレイヤーを感知した敵全体
    private static List<GameObject> gridEnemies; //グリッドに登録された敵
    private static List<GameObject> canAttackEnemies; //attackQueueで取り出された敵を一時保管する
	private static List<GameObject> attackQueue; //攻撃する順番
    private static Dictionary<GameObject, Vector3> ene_pos_listMap; //gridEnemyとその登録座標の対応付け連想配列

	private static int currentAttackCount;
	private static int attackCount = 3;     //同時に攻撃できる上限
    private static int gridNum = 8;         //グリッドの数
    private Vector3[] gridPositions;        //今フレームのグリッドそれぞれの座標
    private Vector3[] pre_gridPositions; //前フレームのグリッドそれぞれの座標

	private static float thresholdAttackTime = 2; //攻撃した後のインターバルタイム
    private static float thresholdAttackedTime = 1; //連続で攻撃できる最大時間
    private float attackTimer;
    private float attackedTimer;
    private bool isAttackedTimer;
    private float attackRadius = 1.3f;

	private Vector3[] belgianCirclePosition; //プレイヤーを中心とした円座標を求める

	//初期化関数
    void Start() {
        findEnemies = new List<GameObject>();
        gridEnemies = new List<GameObject>();
        canAttackEnemies = new List<GameObject>();
        ene_pos_listMap = new Dictionary<GameObject, Vector3>(gridNum);
        attackQueue = new List<GameObject>();
        currentAttackCount = 0;
        gridPositions = new Vector3[gridNum];
        pre_gridPositions = new Vector3[gridNum];
        attackTimer = 0;
        attackedTimer = 0;
        isAttackedTimer = false;
        belgianCirclePosition = new Vector3[gridNum];
        float radian = 0;
        for (int i = 0; i < gridNum; i++) {
            belgianCirclePosition[i] = new Vector3(Mathf.Cos(radian) * attackRadius, 0, Mathf.Sin(radian) * attackRadius);
            gridPositions[i] = player.position + belgianCirclePosition[i];
            radian += ((2 * Mathf.PI) / gridNum);
        }
    }

	//更新関数
    void Update() {
        //リストの整理
        AdjustList();

        ComputeGridPositionAndRelation();

        EnqueueOrDequeueForAttackQueue();

        AttackPlayerAndRelation();
    }

	//エネミーの目的座標を計算する関数
	private void ComputeGridPositionAndRelation() {

		//前フレームのgridPositionを保存
		for (int i = 0; i < gridNum; i++)
			pre_gridPositions[i] = gridPositions[i];

		//gridPositionを更新
		for (int i = 0; i < gridNum; i++) {
			gridPositions[i] = player.position + belgianCirclePosition[i];
		}

		//eneToPosとposToEneに使っているgriPosも更新
		foreach (GameObject gridEnemy in gridEnemies) { 
			for (int j = 0; j < gridNum; j++) {
				if (ene_pos_listMap[gridEnemy] != pre_gridPositions[j]) continue;
				ene_pos_listMap[gridEnemy] = gridPositions[j];
				break;
			}
		}
	}

	//gridEnemiesが登録した座標に到着した順からattackQueueに登録する関数
	private void EnqueueOrDequeueForAttackQueue() {

		foreach (GameObject gridEnemy in gridEnemies) { 
			Vector3 curDis = gridEnemy.transform.position - ene_pos_listMap[gridEnemy];
			curDis.y = 0;
			if (curDis.magnitude < 0.1f) { //登録座標との一致度
				if (!attackQueue.Contains(gridEnemy))
					//                    attackQueue.AddLast(gridEnemy); //Enqueue
					attackQueue.Add(gridEnemy);
			} else {
				if (attackQueue.Contains(gridEnemy))
					attackQueue.Remove(gridEnemy);
			}
		}
	}

	//攻撃と次の準備関数
	private void AttackPlayerAndRelation() {
		//攻撃してから少しの間だけ、指定した数の敵だけ連続で攻撃できる
		if (isAttackedTimer) {
			attackedTimer += Time.deltaTime;
			if (attackedTimer > thresholdAttackedTime || currentAttackCount >= attackCount) {
				isAttackedTimer = false;
				attackedTimer = 0;
				attackTimer = 0;
				currentAttackCount = 0;
			}
		}

		attackTimer += Time.deltaTime;
		if (attackTimer <= thresholdAttackTime) return;
		attackTimer = 1 << 30;
		if (attackQueue.Count == 0) return;
		isAttackedTimer = true;
		currentAttackCount++;

		//攻撃許可をもらう敵
		GameObject attackEnemy = attackQueue[0];
		attackQueue.RemoveAt(0);

		RemoveFromBelgianAI(attackEnemy); //belgianAIの諸々から外す                
		canAttackEnemies.Add(attackEnemy);

		//findEnemiesの中から既に登録中の敵と攻撃中の敵以外を探す(その中から選ぶから)
		//YFNG = YesFind and NoGrid
		List<GameObject> YFNG_Enemy = new List<GameObject>();
		foreach (GameObject findEnemy in findEnemies) {
			if (gridEnemies.Contains(findEnemy)) continue;
			if (canAttackEnemies.Contains(findEnemy)) continue;
			YFNG_Enemy.Add(findEnemy);
		}
		if (YFNG_Enemy.Count <= 0) return; //いないなら終了

		//敵それぞれで一番距離が近いやつを入れる
		SortedDictionary<float, GameObject> dis_ene_map = new SortedDictionary<float, GameObject>();
		for (int YFNG_num = 0; YFNG_num < YFNG_Enemy.Count; YFNG_num++) {
			float distance = (1 << 30);
			for (int griPosNum = 0; griPosNum < gridPositions.Length; griPosNum++) {
				float will_minDis = (YFNG_Enemy[YFNG_num].transform.position - gridPositions[griPosNum]).magnitude;
				distance = distance < will_minDis ? distance : will_minDis;
			}
			//同じ距離が複数あるならその内のひとつを選べばいい
			dis_ene_map[distance] = YFNG_Enemy[YFNG_num];
		}

		//追加するのは一番近い敵だけ
		foreach (GameObject bestEnemy in dis_ene_map.Values) {
			GameObject willAddEnemy = bestEnemy;
			AddGridEnemies(willAddEnemy);
			break;
		}
	}


	//リストの更新関数
	private void AdjustList() {

		//gridEnemyの整理
		for (int i = gridEnemies.Count - 1; i >= 0; i--) {
			if (gridEnemies[i] == null)
				gridEnemies.RemoveAt(i);
		}

		//findEnemyの整理
		for (int i = findEnemies.Count - 1; i >= 0; i--) {
			if (findEnemies[i] == null)
				findEnemies.RemoveAt(i);
		}

		//canAttackEnemiesの整理
		for (int i = canAttackEnemies.Count - 1; i >= 0; i--) {
			if (canAttackEnemies[i] == null)
				canAttackEnemies.RemoveAt(i);
		}

		//attackQueueの整理
		for (int i = attackQueue.Count - 1; i >= 0; i--) {
			if (attackQueue[i] == null)
				attackQueue.RemoveAt(i);
		}
	}


    //プレイヤー発見エネミーリストに追加する
    public void AddFindEnemies(GameObject enemy) {
        findEnemies.Add(enemy);
        AddGridEnemies(enemy);
    }

	//エネミーをBelgianAIに追加する関数
    public void AddGridEnemies(GameObject enemy) {
        //findEnemiesの中からgridEnemiesに登録できるだけ登録する
        if (gridEnemies.Count >= gridNum) return;
        //        gridEnemies.AddLast(enemy)
        gridEnemies.Add(enemy);

        //距離とgridPositionを対応付ける
        SortedDictionary<float, Vector3> dis_griPos_map = new SortedDictionary<float, Vector3>();
        for (int i = 0; i < gridPositions.Length; i++) {
            float distance = (enemy.transform.position - gridPositions[i]).magnitude;
            dis_griPos_map[distance] = gridPositions[i];
        }

        //一番距離が近いgriPosにenemyを登録
        foreach (Vector3 bestGriPos in dis_griPos_map.Values) {
            if (ene_pos_listMap.ContainsValue(bestGriPos)) continue;
            ene_pos_listMap[enemy] = bestGriPos;
            break;
        }
    }

	//BelgianAIからエネミーを削除する関数
	public void RemoveFromBelgianAI(GameObject enemy) {
		if (findEnemies.Contains(enemy)) //findEnemy
			findEnemies.Remove(enemy);
		if (gridEnemies.Contains(enemy)) //gridEnemy
			gridEnemies.Remove(enemy);
		if (ene_pos_listMap.ContainsKey(enemy)) //ene_pos_listMap
			ene_pos_listMap.Remove(enemy);
		if (canAttackEnemies.Contains(enemy)) //canAttackEne
			canAttackEnemies.Remove(enemy);
		if (attackQueue.Contains(enemy)) //attackQueue
			attackQueue.Remove(enemy);
	}

    //攻撃できるかどうかを聞かれる関数
    public bool CanAttackPlayer(GameObject enemy) {
        //canAttackEnemiesにあるならtrueを返してcanAttackEnemiesから削除
        if(canAttackEnemies.Contains(enemy)) {
            canAttackEnemies.Remove(enemy);
            return true;
        } 
        return false;
    }

    //gridEnemiesに登録されているかを聞かれる関数
    public bool IsInGridEnemies(GameObject enemy) {
        return gridEnemies.Contains(enemy);
    }

    //findEnemiesに登録されているかを聞かれる関数
    public bool IsInFindEnemies(GameObject enemy) {
        return findEnemies.Contains(enemy);
    }

	//エネミーが目的座標に到着したかの判定関数
    public bool IsInCloseToGriPos(GameObject enemy) {
        return attackQueue.Contains(enemy);
    }

    //その敵が登録されている座標を返す
    public Vector3 ComputeRegisteredGridPosition(GameObject enemy) {
        if (ene_pos_listMap.ContainsKey(enemy))
            return ene_pos_listMap[enemy];
        return Vector3.zero;
    }

}
