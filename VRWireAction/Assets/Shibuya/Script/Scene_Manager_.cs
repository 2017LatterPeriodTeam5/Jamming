using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager_ : MonoBehaviour {

    //非同期コルーチン
    private AsyncOperation async_;
    //読み込み中か
    [SerializeField]
    private bool loadingNow = false;
    //シーン読み込み待ち一覧
    private List<string> waitLoadingScene = new List<string>();
    //シーンの一覧
    public enum SceneList
    {
        TitleScene,        //タイトルシーン
        BaseScene,         //スクリプト管理シーン
        GameScene,         //ゲームシーン
        ResultScene,       //リザルトシーン
        Shibuya,           //テスト用シーン
    }

	void Start () {
        //タイトル表示
        SceneAdd(SceneList.TitleScene);
	}
	
	void Update (){
        //テストシーン追加と削除
        if (Input.GetKeyDown(KeyCode.U)) SceneAdd(SceneList.Shibuya);
        if (Input.GetKeyDown(KeyCode.I)) SceneAdd(SceneList.GameScene);
        if (Input.GetKeyDown(KeyCode.O)) SceneAdd(SceneList.ResultScene);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SceneRemove(SceneList.Shibuya);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SceneRemove(SceneList.GameScene);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SceneRemove(SceneList.ResultScene);
        if (Input.GetKeyDown(KeyCode.K)) SceneRemove(SceneList.TitleScene);
        //読み込み待ちがあれば読み込む
        if(waitLoadingScene.Count != 0)
        {
            WaitLoadingScene();
        }
    }
    //シーン読み込み予約の処理
    private void WaitLoadingScene()
    {
        foreach (string sceneName in waitLoadingScene)
        {
            //他のシーンが読み込み中でなければ
            if(!loadingNow)
            {
                //シーンの読み込みを始めたら読み込み待ち一覧から削除
                waitLoadingScene.Remove(sceneName);
                //シーンの読み込み
                StartCoroutine(LoadScene(sceneName));
                break;
            }
        }
    }
    //シーン読み込み予約一覧に追加
    public void SceneAdd(SceneList sceneList)
    {
        //同じ名前のシーンがあるか
        if (!SameScene(sceneList.ToString()) )
        {//無ければ追加
            waitLoadingScene.Add(sceneList.ToString());
        }
    }
    //シーン読み込み
    private IEnumerator LoadScene(string sceneName)
    {
        //シーンを追加
        async_ = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //シーンが完全に読み込めるまで非アクティブ
        async_.allowSceneActivation = false;
        //読み込み中
        loadingNow = true;
        //シーンの読み込み完了までループ
        while (async_.progress < 0.9f)
        {
            yield return 0;
        }
        //読み込み完了後0.1秒待つ
        yield return new WaitForSeconds(0.1f);
        //シーンの読み込み完了したらアクティブ
        async_.allowSceneActivation = true;
        //読み込み完了
        loadingNow = false;

        yield return async_;
    }
    //シーン削除
    public void SceneRemove(SceneList sceneList)
    {
        //同じ名前のシーンがあるか
        if (SameScene(sceneList.ToString()))
        {//あれば削除
            SceneManager.UnloadSceneAsync(sceneList.ToString());
        }
    }
    //現在表示しているシーンに同じ名前のシーンがないか確認
    bool SameScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    //読み込み中かどうか
    public bool IsLoading()
    {
        return loadingNow;
    }
}
