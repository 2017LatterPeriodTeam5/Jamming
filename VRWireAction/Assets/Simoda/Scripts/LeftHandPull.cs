using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandPull : MonoBehaviour {

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("左手")]
    private Transform m_LeftHand;

    /*==内部設定変数==*/
    //過去の手の情報
    private HandInfo m_PrevHandInfo = new HandInfo();
    //現在の手の情報
    private HandInfo m_CurrHandInfo = new HandInfo();

    //手を引いているかどうか
    private bool m_Pull = false;
    //時間
    private float m_TotalTime;


    void Start ()
    {
        //過去の手の情報を設定
        m_PrevHandInfo.pos = m_LeftHand.position;
        m_PrevHandInfo.localPos = m_LeftHand.localPosition;
        m_PrevHandInfo.forward = m_LeftHand.forward;
    }
	
	void Update ()
    {
        m_Pull = false;

        m_TotalTime += Time.deltaTime;

        m_PrevHandInfo.pos = m_CurrHandInfo.pos;
        m_PrevHandInfo.localPos = m_CurrHandInfo.localPos;
        m_PrevHandInfo.forward = m_CurrHandInfo.forward;

        if (m_TotalTime >= 0.3f)
        {

            m_CurrHandInfo.pos = m_LeftHand.position;
            m_CurrHandInfo.localPos = m_LeftHand.localPosition;
            m_CurrHandInfo.forward = m_LeftHand.forward;

            Vector3 prevBack = -m_PrevHandInfo.forward;
            Vector3 prevPosFromCurrPos = (m_CurrHandInfo.localPos - m_PrevHandInfo.localPos);
            Vector3 prevPosFromCurrPosNor = (m_CurrHandInfo.localPos - m_PrevHandInfo.localPos).normalized;

            float value = (prevBack.x - prevPosFromCurrPosNor.x) + (prevBack.z - prevPosFromCurrPosNor.z);
            if (value < 0.2f && value > -0.2f && prevPosFromCurrPos.magnitude * 1000.0f > 4.0f)
            {
                m_Pull = true;
            }

            m_TotalTime = 0.0f;
        }
    }

    public bool GetTouchPull()
    {
        return m_Pull;
    }
}
