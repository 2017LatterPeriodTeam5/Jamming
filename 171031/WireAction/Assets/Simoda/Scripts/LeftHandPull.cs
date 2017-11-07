using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandPull : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private PlayerInfo m_PlayerInfo;

    /*==内部設定変数==*/
    //過去の手の情報
    private HandInfo m_PrevHandInfo = new HandInfo();
    //現在の手の情報
    private HandInfo m_CurrHandInfo = new HandInfo();
    //手を引いているかどうか
    private bool m_Pull = false;
    //合計時間
    private float m_TotalTime;
    public void Awake()
    {
        //コンポーネント取得
        m_PlayerInfo = GetComponent<PlayerInfo>();
    }

    void Start()
    {
        //過去の手の情報を設定
        m_PrevHandInfo.pos = m_PlayerInfo.GetLeftHand().position;
        m_PrevHandInfo.localPos = m_PlayerInfo.GetLeftHand().localPosition;
        m_PrevHandInfo.forward = m_PlayerInfo.GetLeftHand().forward;

        //変数のリセット
        m_Pull = false;
        m_TotalTime = 0.0f;
    }

    void Update()
    {
        m_Pull = false;

        m_TotalTime += Time.deltaTime;

        //過去の手の情報を更新
        m_PrevHandInfo.pos = m_CurrHandInfo.pos;
        m_PrevHandInfo.localPos = m_CurrHandInfo.localPos;
        m_PrevHandInfo.forward = m_CurrHandInfo.forward;

        if (m_TotalTime >= m_PlayerInfo.GetPullTime())
        {
            //現在の手の情報を更新
            m_CurrHandInfo.pos = m_PlayerInfo.GetLeftHand().position;
            m_CurrHandInfo.localPos = m_PlayerInfo.GetLeftHand().localPosition;
            m_CurrHandInfo.forward = m_PlayerInfo.GetLeftHand().forward;

            //過去の手の後ろ方向を設定
            Vector3 prevBack = -m_PrevHandInfo.forward;
            //正規化した過去の手の位置から現在の手の位置への方向
            Vector3 prevPosFromCurrPosNor = (m_CurrHandInfo.localPos - m_PrevHandInfo.localPos).normalized;

            //過去の手から現在の手までの距離
            float dis = Vector3.Distance(m_CurrHandInfo.localPos, m_PrevHandInfo.localPos) * 10.0f;
            //過去の手の後ろ方向と、過去の手の位置から現在の手の位置への方向がどれくらい違っているか
            float value = (prevBack.x - prevPosFromCurrPosNor.x) + (prevBack.z - prevPosFromCurrPosNor.z);

            if (value < 0.2f && value > -0.2f
                && dis > m_PlayerInfo.GetPullDistance())
            {
                m_Pull = true;
            }

            //合計時間をリセット
            m_TotalTime = 0.0f;
        }
    }

    /**==============================================================================================*/
    /** 外部から使用する
    /**==============================================================================================*/

    /// <summary>
    /// 手を引いたかどうかを返す
    /// </summary>
    /// <returns>手を引いたかどうか</returns>
    public bool GetTouchPull()
    {
        return m_Pull;
    }
}
