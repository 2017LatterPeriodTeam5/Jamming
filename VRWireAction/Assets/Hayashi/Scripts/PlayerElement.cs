using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElement : MonoBehaviour
{
    public Transform initRestartPoint; //処理リスタートポイント
    private Transform m_RestartPoint;   //自分のリスタートポイント
    private Scene_Manager_ sceneManager;
    private Scene_Manager_Fade sceneManager_Fade;
    private TimeLimit timeLimit;

    public GameObject player;   //プレイヤー

    // Use this for initialization
    void Start()
    {
        sceneManager = GameObject.Find("ScriptManager").GetComponent<Scene_Manager_>();
        sceneManager_Fade = Camera.main.GetComponent<Scene_Manager_Fade>();
        m_RestartPoint = initRestartPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //リスタートポイントの更新
    public void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "RestartPoint":
                m_RestartPoint = other.transform;
                break;

            case "DestroyWall":
                player.transform.position = m_RestartPoint.position;
                break;

            case "GoalPoint":
                sceneManager.ChangeSceneState("ResultScene");
                break;

        }
    }
}
