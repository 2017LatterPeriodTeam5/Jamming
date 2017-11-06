using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class TagNameClassCreator
{
    //コマンド名
    private const string COMMAND_NAME = "Tools/Create/TagName Class";

    //タグをまとめるDictionary
    private static Dictionary<string, string> _tagsDict;

    //タグを管理するクラスを作成します
    [MenuItem(COMMAND_NAME)]
    private static void CreatePathClass()
    {
        //ディクショナリー初期化
        _tagsDict = new Dictionary<string, string>();

        //全てのタグを取得
        string[] tags = InternalEditorUtility.tags;
        foreach (string tag in tags)
        {
            string tagName = tag;
            _tagsDict[tagName] = tag;
        }

        //定数クラス作成
        ConstantsClassCreator.Create("TagName", "タグを定数で管理するクラス", _tagsDict);
    }
}
