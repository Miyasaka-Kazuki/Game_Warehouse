using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラクターの状態に上書きまたは足し算するための仲介クラス
public class CharacterAdapter : MonoBehaviour {
    CharacterObserver charOb;
    List<CharacterDirector> curRegisterList; //ギミックに接触しているキャラクターのリスト

    void Start() {
        charOb = GetComponent<CharacterObserver>();
        curRegisterList = new List<CharacterDirector>();
    }

	//キャラクターがギミックに接触した瞬間
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
            curRegisterList.Add(other.gameObject.GetComponent<CharacterDirector>());
    }

	//キャラクターがギミックから離れた瞬間
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy") {
            curRegisterList.Remove(other.gameObject.GetComponent<CharacterDirector>());
            //離れるときにまだ登録されてるなら自動解除
            other.gameObject.GetComponent<CharacterDirector>().RemoveObserver(this.charOb);
        }
    }

    //リストにあるすべてのオブジェクトを登録
    public void RegisterCharaDirector() {
        foreach (CharacterDirector cd in curRegisterList) {
            cd.AddObserver(this.charOb);
        }
    }
    //リストにあるすべてのオブジェクトを削除
    public void ReleaseCharaDirector() {
        foreach (CharacterDirector cd in curRegisterList) {
            cd.RemoveObserver(this.charOb);
        }
    }

	//登録オブジェクトの数を取得
    public  int GetCurRegisterListCount() {
        return curRegisterList.Count;
    }
}
