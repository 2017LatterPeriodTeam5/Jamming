using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElement : MonoBehaviour
{
    public Transform initRestartPoint; //処理リスタートポイント
    private Transform m_RestartPoint;   //自分のリスタートポイント
    private Scene_Manager_ sceneManager;    //シーンマネージャー
    private Scene_Manager_Fade sceneManager_Fade;   　//フェードシーンマネージャー
    private TimeLimit timeLimit;    //制限時間

    public GameObject player;   //プレイヤー

    // Use this for initialization
    void Start()
    {
        sceneManager = GameObject.Find("ScriptManager").GetComponent<Scene_Manager_>();
        sceneManager_Fade = Camera.main.GetComponent<Scene_Manager_Fade>();
        timeLimit = GameObject.Find("TimeTex").GetComponent<TimeLimit>();
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
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<VR_PlayerWireAction>().JointConnectedBodyRelease();
                GetComponent<VR_PlayerWireAction>().WireLineDelete();
                break;

            case "DestroyWall":
                player.transform.position = m_RestartPoint.position;
                break;

            case "GoalPoint":
                sceneManager.ChangeSceneState("ResultScene");
                timeLimit.CountStop();
                break;

        }
    }
}
