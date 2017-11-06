using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager_Fade : MonoBehaviour
{

    public float fadeTime = 2.0f;   //フェード時間
    public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);  //フェードの色
    public Shader fadeShader = null;    //フェードシェーダー

    private Scene_Manager_ sceneManager;
    private Material fadeMaterial = null;   //フェードマテリアル
    private bool isFading = false;  //フェードしているか?


    void Awake()
    {
        sceneManager = GameObject.Find("ScriptManager").GetComponent<Scene_Manager_>();
        fadeMaterial = (fadeShader != null) ? new Material(fadeShader) : new Material(Shader.Find("Transparent/Diffuse"));
    }

    public void OnEnable()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadSceenWithFade(string sceneName)
    {       
        StartCoroutine(FadeOut(sceneName));
    }

    public void SceneRemove(SceneList sceneList)
    {
        //同じ名前のシーンがあるか
        if (SameScene(sceneList.ToString()))
        {//あれば削除
            SceneManager.UnloadSceneAsync(sceneList.ToString());
        }
    }

    public void OnDestroy()
    {
        if (fadeMaterial != null)
        {
            Destroy(fadeMaterial);
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        Color color = fadeMaterial.color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        isFading = true;
    }

    IEnumerator FadeOut(string sceneName)
    {
        float elapsedTime = 0.0f;
        fadeColor.a = 0f;
        Color color = fadeMaterial.color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            color.a = 0.0f + Mathf.Clamp01(elapsedTime / fadeTime);
            fadeMaterial.color = color;
        }
        sceneManager.ChangeSceneState(sceneName);
        isFading = false;
    }

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

    void OnPostRender()
    {
        if (isFading)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(fadeMaterial.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }

}
