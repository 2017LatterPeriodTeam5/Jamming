using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInfo
{
    public Vector3 pos;
    public Vector3 localPos;
    public Vector3 forward;
}

public class TouchAcceleration : MonoBehaviour
{

    public Transform m_Touch;

    private Vector3 m_PrevPos;
    private Vector3 m_CurrPos;
    private float m_FrameTime;

    private float m_Acceleration;
    private float m_TotalTime;

    public TouchInfo m_PrevTouchInfo = new TouchInfo();
    public TouchInfo m_CurrTouchInfo = new TouchInfo();
    private bool m_Pull = false;

    // Use this for initialization
    void Start()
    {
        m_PrevPos = m_Touch.forward;
        // m_CurrPos = m_Touch.localPosition;

        m_PrevTouchInfo.pos = m_Touch.position;
        m_PrevTouchInfo.localPos = m_Touch.localPosition;
        m_PrevTouchInfo.forward = m_Touch.forward;

    }

    // Update is called once per frame
    void Update()
    {
        m_Pull = false;

        m_TotalTime += Time.deltaTime;

        m_PrevTouchInfo.pos = m_CurrTouchInfo.pos;
        m_PrevTouchInfo.localPos = m_CurrTouchInfo.localPos;
        m_PrevTouchInfo.forward = m_CurrTouchInfo.forward;

        if (m_TotalTime >= 0.3f)
        {

            m_CurrTouchInfo.pos = m_Touch.position;
            m_CurrTouchInfo.localPos = m_Touch.localPosition;
            m_CurrTouchInfo.forward = m_Touch.forward;

            //Debug.Log(-m_PrevTouchInfo.forward + ":" + (m_CurrTouchInfo.localPos - m_PrevTouchInfo.localPos).normalized);
            Vector3 prevBack = -m_PrevTouchInfo.forward;
            Vector3 prevPosFromCurrPos = (m_CurrTouchInfo.localPos - m_PrevTouchInfo.localPos);
            Vector3 prevPosFromCurrPosNor = (m_CurrTouchInfo.localPos - m_PrevTouchInfo.localPos).normalized;

            float value = (prevBack.x - prevPosFromCurrPosNor.x) + (prevBack.z - prevPosFromCurrPosNor.z);
            //Debug.Log( Mathf.Floor(prevPosFromCurrPos.magnitude * 1000));
            if (value < 0.2f && value > -0.2f && prevPosFromCurrPos.magnitude * 1000.0f > 4.0f)
            {
                m_Pull = true;
            }

            //float pullValue = (m_PrevTouchInfo.forward.x - m_CurrTouchInfo.forward.x) + (m_PrevTouchInfo.forward.z - m_CurrTouchInfo.forward.z);
            //if (pullValue < 0.2f && pullValue > -0.2f)
            //{
            //    float prevDis = Vector3.Distance(transform.position, m_PrevTouchInfo.pos);
            //    float currDis = Vector3.Distance(transform.position, m_CurrTouchInfo.pos);

            //    Debug.Log(Vector3.Distance(m_PrevTouchInfo.pos, m_CurrTouchInfo.pos));
            //    if (Vector3.Distance(m_PrevTouchInfo.localPos, m_CurrTouchInfo.localPos) > 0.25f && currDis < prevDis)
            //    {
            //        Debug.Log("現在" + currDis + "　過去" + prevDis);
            //        Debug.Log(-m_PrevTouchInfo.forward + ":" + (m_CurrTouchInfo.localPos - m_PrevTouchInfo.localPos).normalized);
            //        m_Pull = true;
            //    }
            //}


            //Debug.Log(Vector3.Angle(-m_PrevTouchInfo.forward, -m_CurrTouchInfo.forward));
            //Debug.Log(-m_PrevTouchInfo.forward + ":" + (m_PrevTouchInfo.pos - m_CurrTouchInfo.pos).normalized);

            m_Acceleration = ((m_CurrPos - m_PrevPos) / m_TotalTime).z;
            m_Acceleration = m_Acceleration / m_TotalTime;
            //print(m_Acceleration);

            m_TotalTime = 0.0f;
            m_PrevPos = m_CurrPos;
        }



        //Debug.Log(m_Touch.forward);
        //if ( m_Acceleration < -500.0f)
        //    print(m_Acceleration);
    }

    public float GetTouchAcceleration()
    {
        return m_Acceleration;
    }

    public bool GetTouchPull()
    {
        return m_Pull;
    }
}
