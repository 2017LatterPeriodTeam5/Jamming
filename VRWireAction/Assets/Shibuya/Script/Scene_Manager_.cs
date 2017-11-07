using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

//シーンの一覧
public enum SceneList
{
    BaseScene,         //スクリプト管理シーン
    TitleScene,        //タイトルシーン
    MenuScene,         //メニューシーン
    GameScene,         //ゲームシーン
    aStage,
    ResultScene,       //リザルトシーン
    GameOverScene,     //ゲームオーバーシーン
}

public class Scene_Manager_ : MonoBehaviour {

    //非同期コルーチン
    private AsyncOperation async_;

    //読み込み中か
    [SerializeField]
    private bool loadingNow = false;

    //クリアしたか？ /*クリア情報はスクリプトが出来たら外部から取得*/ 
    [SerializeField]
    private bool is_Clear = false;

    //リトライか？   /*リトライ情報はスクリプトが出来たら外部から取得*/
    [SerializeField]
    private bool is_Retry = false;

    //シーン読み込み待ち一覧
    private List<string> waitLoadingScene = new List<string>();

    //音楽管理
    private SoundsManager soundsManager_;

    //現在のシーン
    private string currentScene_;

	void Start () {
        soundsManager_ = GetComponent<SoundsManager>();
        //現在のシーン  /*タイトル*/
        currentScene_ = SceneList.TitleScene.ToString();
        //タイトル表示
        ChangeSceneState(currentScene_);
    }
	
	void Update (){
        //テストシーン遷移
        if (Input.GetKeyDown(KeyCode.O)) NextScene();
        if (Input.GetKeyDown(KeyCode.P)) is_Clear = true;
        if (Input.GetKeyDown(KeyCode.Alpha0)) is_Retry = true;
        //読み込み待ちがあれば読み込む
        if (waitLoadingScene.Count != 0)
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
    private void SceneAdd(SceneList sceneList)
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
    private void SceneRemove(SceneList sceneList)
    {
        //同じ名前のシーンがあるか
        if (SameScene(sceneList.ToString()))
        {//あれば削除
            SceneManager.UnloadSceneAsync(sceneList.ToString());
        }
    }
    //*********(簡易版)次のシーンへ*********
    public void NextScene()
    {
        switch(currentScene_)
        {
            case "TitleScene":
                currentScene_ = SceneList.MenuScene.ToString();
                break;
            case "MenuScene":
                currentScene_ = SceneList.GameScene.ToString();
                break;
            case "GameScene":
                if (is_Clear)
                {
                    is_Clear = false;
                    currentScene_ = SceneList.ResultScene.ToString();
                }
                else if (!is_Clear)
                {
                    currentScene_ = SceneList.GameOverScene.ToString();
                }
                break;
            case "GameOverScene":
                if (is_Retry)
                {
                    is_Retry = false;
                    currentScene_ = SceneList.GameScene.ToString();
                }
                else if (!is_Retry)
                {
                    currentScene_ = SceneList.TitleScene.ToString();
                }
                break;
            case "ResultScene":
                currentScene_ = SceneList.TitleScene.ToString();
                break;
        }
        ChangeSceneState(currentScene_);
    }
    //シーン状態の変更   /*シーン状態に応じて各シーンの追加や削除を行う。なんとなくBGMだけ音楽管理する*/
    public void ChangeSceneState(string currentScene)
    {
        switch (currentScene)
        {
            case "TitleScene":
                soundsManager_.StopBGM();
                SceneRemove(SceneList.MenuScene);
                SceneRemove(SceneList.GameScene);
                SceneRemove(SceneList.GameOverScene);
                SceneRemove(SceneList.ResultScene);
                SceneAdd(SceneList.TitleScene);
                break;
            case "MenuScene":
                soundsManager_.StopBGM();
                SceneRemove(SceneList.TitleScene);
                SceneRemove(SceneList.GameScene);
                SceneRemove(SceneList.GameOverScene);
                SceneRemove(SceneList.ResultScene);
                SceneAdd(SceneList.MenuScene);
                break;
            case "GameScene":
                soundsManager_.StopBGM();
                SceneRemove(SceneList.TitleScene);
                SceneRemove(SceneList.MenuScene);
                SceneRemove(SceneList.GameOverScene);
                SceneRemove(SceneList.ResultScene);
                SceneAdd(SceneList.GameScene);
                break;
            case "aStage":
                soundsManager_.StopBGM();
                SceneRemove(SceneList.TitleScene);
                SceneRemove(SceneList.MenuScene);
                SceneRemove(SceneList.GameOverScene);
                SceneRemove(SceneList.ResultScene);
                SceneAdd(SceneList.aStage);
                break;
            case "GameOverScene":
                soundsManager_.StopBGM();
                SceneAdd(SceneList.GameOverScene);
                break;
            case "ResultScene":
                soundsManager_.StopBGM();
                SceneRemove(SceneList.TitleScene);
                SceneRemove(SceneList.MenuScene);
                SceneRemove(SceneList.GameOverScene);
                SceneAdd(SceneList.ResultScene);
                break;
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
        if(waitLoadingScene.Count != 0 || loadingNow)
        {
            return true;
        }
        return false;
    }
}
