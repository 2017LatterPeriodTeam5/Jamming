using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandPull : MonoBehaviour
{
    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("右手")]
    private Transform m_RightHand;

    /*==内部設定変数==*/
    //過去の手の情報
    private HandInfo m_PrevHandInfo = new HandInfo();
    //現在の手の情報
    private HandInfo m_CurrHandInfo = new HandInfo();

    //手を引いているかどうか
    private bool m_Pull = false;
    //時間
    private float m_TotalTime;

    void Start()
    {
        //過去の手の情報を設定
        m_PrevHandInfo.pos = m_RightHand.position;
        m_PrevHandInfo.localPos = m_RightHand.localPosition;
        m_PrevHandInfo.forward = m_RightHand.forward;
    }

    void Update()
    {
        m_Pull = false;

        m_TotalTime += Time.deltaTime;

        m_PrevHandInfo.pos = m_CurrHandInfo.pos;
        m_PrevHandInfo.localPos = m_CurrHandInfo.localPos;
        m_PrevHandInfo.forward = m_CurrHandInfo.forward;

        if (m_TotalTime >= 0.3f)
        {

            m_CurrHandInfo.pos = m_RightHand.position;
            m_CurrHandInfo.localPos = m_RightHand.localPosition;
            m_CurrHandInfo.forward = m_RightHand.forward;

            Vector3 prevBack = -m_PrevHandInfo.forward;
            Vector3 prevPosFromCurrPos = (m_CurrHandInfo.localPos - m_PrevHandInfo.localPos);
            Vector3 prevPosFromCurrPosNor = (m_CurrHandInfo.localPos - m_PrevHandInfo.localPos).normalized;


            float dis = Vector3.Distance(m_CurrHandInfo.localPos, m_PrevHandInfo.localPos) * 10.0f;

            float value = (prevBack.x - prevPosFromCurrPosNor.x) + (prevBack.z - prevPosFromCurrPosNor.z);
            if (value < 0.2f && value > -0.2f && dis > 1.5f)
            {
                m_Pull = true;
                Debug.Log(dis);
            }

            m_TotalTime = 0.0f;
        }
    }

    public bool GetTouchPull()
    {
        return m_Pull;
    }
}
