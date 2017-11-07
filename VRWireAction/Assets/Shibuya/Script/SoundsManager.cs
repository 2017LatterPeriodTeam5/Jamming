using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using UnityEditor;

public enum SoundType
{
    bgm,
    se
}

public class SoundsManager : MonoBehaviour {

    //フェード時間
    private float fadeTime;
    //フェード時間の初期値
    private float initialFadeTime;
    //フェードイン・アウト
    private bool fadeIn;
    //フェードイン・アウトの実行状態
    private bool fadeActivate;

    //音源
    private AudioSource bgm_Source_;
    private AudioSource se_Source_;
    private AudioSource[] se_Source_Channel;
    //チャンネル数
    private int se_Channel;
    //音楽ファイルへのパス
    private string folderPass_;
    //BGMとSEのテーブル
    private Dictionary<string, SoundData>  bgm_Table_ = new Dictionary<string, SoundData>();
    private Dictionary<string, SoundData> se_Table_ = new Dictionary<string, SoundData>();

    void Awake()
    {
        //フェード関係
        fadeActivate = false;
        fadeIn = false;

        //オーディオソース関係
        se_Channel = 5;
        se_Source_Channel = new AudioSource[se_Channel];
        folderPass_ = "Assets/Shibuya/Resources/";

        ////テスト音源読み込み
        //LoadSound("testmusic", SoundType.bgm);
        //LoadSound("testse", SoundType.se);
        //LoadSound("testse2", SoundType.se);
    }

    void Update()
    {
        //フェード処理
        if (fadeActivate) FadeInOutCount(fadeTime, fadeIn);
    }

    //音楽ファイルの読み込み 
    public void LoadSound(string soundName,SoundType soundType)
    {
        //音楽タイプで振り分け
        if(soundType == SoundType.bgm)
        {
            bgm_Table_.Add(soundName, new SoundData(soundName,folderPass_));
        }
        else if(soundType == SoundType.se)
        {
            se_Table_.Add(soundName, new SoundData(soundName, folderPass_));
        }
    }
    //BGM再生
    public void PlayBGM(string soundName)
    {
        //音楽読み込みがされていない場合のエラーメッセージ
        if (bgm_Table_.ContainsKey(soundName) == false)
        {
            Debug.Log("SoundsManagerクラスのPlayBGMメソッドで指定された" + soundName + "は存在しないか、LoadSoundで読み込まれていません");
            return;
        }

        var soundData_ = bgm_Table_[soundName];

        //SoundTypeがBGMの時はチャンネルを０に指定(一応０以外でも動く)
        bgm_Source_ = GetAudioSource(SoundType.bgm,0);
        bgm_Source_.loop = true;
        bgm_Source_.clip = soundData_.clip;
        bgm_Source_.Play();
    }
    //効果音再生     /*チャンネル分け可*/
    public void PlaySE(string soundName, int channel)
    {
        //音楽読み込みがされていない場合のエラーメッセージ
        if (se_Table_.ContainsKey(soundName) == false)
        {
            Debug.Log("SoundsManagerクラスのPlaySEメソッドで指定された" + soundName + "は存在しないか、LoadSoundで読み込まれていません");
            return;
        }

        var soundData_ = se_Table_[soundName];

        se_Source_ = GetAudioSource(SoundType.se, channel);
        se_Source_.playOnAwake = false;
        se_Source_.PlayOneShot(soundData_.clip);
    }
    //AudioSourceの生成と返す音楽データの選別
    private AudioSource GetAudioSource(SoundType soundType,int channel)
    {
        //コンポネントにAudioSourceがなければ追加
        if(this.gameObject.GetComponent<AudioSource>() == null)
        {
            bgm_Source_ = gameObject.AddComponent<AudioSource>();
            se_Source_ = gameObject.AddComponent<AudioSource>();
            for(int i = 0;i < se_Channel; i++)
            {
                se_Source_Channel[i] = gameObject.AddComponent<AudioSource>();
            }
        }

        //音楽タイプとチャンネルの振り分け
        if (soundType == SoundType.bgm)
        {
            return bgm_Source_;
        }
        else if (soundType == SoundType.se)
        {
            if(0 < channel && channel <= se_Channel)
            {
                return se_Source_Channel[channel];
            }
            else
            {
                return se_Source_;
            }
        }
        return null;
    }
    //音楽停止
    public void StopBGM()
    {
        if(bgm_Source_ != null)bgm_Source_.Stop();
    }

    //フェードの実行準備 
    public void FadeInOutActivate(float fadeTime,bool fadeIn)
    {
        fadeActivate = true;
        initialFadeTime = this.fadeTime = fadeTime * 60.0f;
        this.fadeIn = fadeIn;
    }
    //フェードのカウント
    private void FadeInOutCount(float fadeTime,bool fadeIn)
    {
        //フェードのカウント
        this.fadeTime -= Time.deltaTime;
        //フェードの開始
        FadeBGM(this.fadeTime, fadeIn, initialFadeTime);
    }
    //フェードの実行
    private void FadeBGM(float fadeTime,bool fadeIn,float initialFadeTime)
    {
        if (fadeIn)
        {//フェードイン
            bgm_Source_.volume += (initialFadeTime - fadeTime) / fadeTime; 
        }
        else if(!fadeIn)
        {//フェードアウト
            bgm_Source_.volume -= (initialFadeTime - fadeTime) / fadeTime;
        }
        //フェード音量限界に達したらフェードを終了
        if (bgm_Source_.volume < 0 || bgm_Source_.volume > 1) fadeActivate = false;
    }
    //BGM音量の値変更
    public void SetVolume(float volume)
    {
        bgm_Source_.volume = volume;
    }
}
