using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//音全体を管理するクラス
public class AudioDirector : MonoBehaviour {
    public AudioSource source_bgm; 
    public AudioSource source_se;
    public AudioClip[] bgm_prefabs;
    public AudioClip[] se_prefabs;
    private string now_bgm_name;
    private string now_se_name;

	//初期化関数
	public void Start() {
        now_bgm_name = null;
        now_se_name = null;
    }
		
	//SEを鳴らす関数
    public void PlaySE(string audio_name, float volume = 1) {
//        if (now_se_name == audio_name) return;
        for (int i = 0; i < se_prefabs.Length; i++) {
            if (!se_prefabs[i].name.Equals(audio_name)) continue; //名前と一致していない
            source_se.PlayOneShot(se_prefabs[i], volume);
            now_se_name = audio_name;
            return;
        }
    }

	//BGMを鳴らす関数
    public void PlayBGM(string audio_name, float volume = 1) {
        if (now_bgm_name == audio_name) return;
        for (int i = 0; i < bgm_prefabs.Length; i++) {
            if (!bgm_prefabs[i].name.Equals(audio_name)) continue; //名前と一致していない
            source_bgm.clip = bgm_prefabs[i];
            source_bgm.Play();
            source_bgm.volume = volume;
            now_bgm_name = audio_name;
            return;
        }
    }
}
