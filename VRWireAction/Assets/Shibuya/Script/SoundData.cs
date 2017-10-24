using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

class SoundData
{
    public string soundName_;
    public string folderPass_;
    public List<string> extension_;
    public AudioClip clip;

    public SoundData(string soundName, string folderPass_)
    {
        this.soundName_ = soundName;
        this.folderPass_ = folderPass_;
        /*対応拡張子*/
        extension_ = new List<string> {".mp3",".wav",".ogg"};
        //指定フォルダからデータを取得
        string[] filePathArray = Directory.GetFiles(folderPass_);

        foreach (string filePath in filePathArray)
        {
            //取得した音楽データ拡張子が対応しているか？
            if (!extension_.Contains(Path.GetExtension(filePath)))
            {
                continue;
            }
            //指定された音楽データの取得
            if (filePath.Contains(folderPass_ + soundName))
            {
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>(filePath);
                break;
            }
        }

        //NULLだったときエラーメッセージ
        if (clip == null)
        {
            Debug.Log("音楽ファイル読み込み失敗。音楽名や拡張子を確認してください");
        }
    }
}
