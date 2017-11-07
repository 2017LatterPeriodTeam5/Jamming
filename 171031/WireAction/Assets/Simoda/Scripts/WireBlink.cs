using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBlink : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private PlayerInfo m_PlayerInfo;
    private VR_PlayerWireAction m_PlayerAction;

    public Transform m_Hand;

    private Quaternion m_PrevRotate;
    private Quaternion m_CurrRotate;

    //時間
    private float m_TotalTime;

    private bool m_HandRotate = false;
    private bool m_BlinkEnd = false;
    private GameObject m_BlinkPointPrefab;
    private GameObject m_BlinkPoint;

    private IEnumerator m_RightWireBlinkWaitCoroutine;
    private IEnumerator m_LeftWireBlinkWaitCoroutine;

    private Vector3 m_BlinkStartPosition;
    private Vector3 m_BlinkingPosition;

    private Transform m_AllBlinkPoint;

    private float m_BlinkRecastTime = 0.0f;

    void Awake()
    {
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_PlayerAction = GetComponent<VR_PlayerWireAction>();

        m_BlinkPointPrefab = Resources.Load(ResourcesFilePath.PREFAB_BLINK_POINT) as GameObject;
    }

    void Start()
    {
        m_PrevRotate = Quaternion.Euler(m_Hand.eulerAngles);

        m_BlinkRecastTime = m_PlayerInfo.GetBlinkRecastTime();
        m_AllBlinkPoint = GameObject.Find("AllBlinkPoint").GetComponent<Transform>();
    }

    void Update()
    {
        m_HandRotate = false;

        m_TotalTime += Time.deltaTime;
        m_BlinkRecastTime += Time.deltaTime;

        m_PrevRotate = Quaternion.Euler(m_CurrRotate.eulerAngles);

        if (m_TotalTime >= m_PlayerInfo.GetTwistTime())
        {
            m_CurrRotate = Quaternion.Euler(m_Hand.eulerAngles);

            float angle = Mathf.Abs(Mathf.DeltaAngle(m_PrevRotate.eulerAngles.z, m_CurrRotate.eulerAngles.z));

            //Debug.Log(angle);

            if (angle >= m_PlayerInfo.GetTwistAngle() && m_BlinkRecastTime >= m_PlayerInfo.GetBlinkRecastTime())
            {
                m_HandRotate = true;
                m_BlinkEnd = false;

                m_BlinkRecastTime = 0.0f;
            }

            m_TotalTime = 0.0f;
        }
    }

    public void WireBlinkWaitStart(Transform hand, Vector3 direction, VR_PlayerWireAction.HandType handType)
    {
        switch (handType)
        {
            case VR_PlayerWireAction.HandType.Right:

                m_Hand = hand;

                m_BlinkStartPosition = transform.position;

                m_RightWireBlinkWaitCoroutine = RightWireBlinkWait(direction);
                StartCoroutine(m_RightWireBlinkWaitCoroutine);

                break;

            case VR_PlayerWireAction.HandType.Left:

                m_Hand = hand;
                m_LeftWireBlinkWaitCoroutine = LeftWireBlinkWait(direction);
                StartCoroutine(m_LeftWireBlinkWaitCoroutine);

                break;
        }

        foreach (Transform blinkPoint in m_AllBlinkPoint)
        {
            Destroy(blinkPoint.gameObject);
        }

        m_BlinkPoint = Instantiate(m_BlinkPointPrefab, m_AllBlinkPoint);
        m_BlinkPoint.transform.position = transform.position + direction * m_PlayerInfo.GetWireDistance();
    }

    private IEnumerator RightWireBlinkWait(Vector3 direction)
    {
        while (!m_HandRotate)
        {
            Debug.Log("WireBlinkWait");

            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                Debug.Log("キャンセル");
                StopCoroutine(m_RightWireBlinkWaitCoroutine);
            }

            yield return null;
        }

        while (!OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            Debug.Log("トリガーが離されるのを待っている");

            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                Debug.Log("キャンセル");
                StopCoroutine(m_RightWireBlinkWaitCoroutine);
            }

            yield return null;
        }

        m_PlayerAction.JointConnectedBodyRelease();

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = direction * m_PlayerInfo.GetBlinkPower();

        while (!m_BlinkEnd)
        {
            if (Vector3.Distance(m_BlinkStartPosition, transform.position) > m_PlayerInfo.GetBlinkInertiaPower())
            {
                m_BlinkEnd = true;
            }
            yield return null;
        }

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(direction * m_PlayerInfo.GetBlinkInertiaPower(), ForceMode.Impulse);
    }

    private IEnumerator LeftWireBlinkWait(Vector3 direction)
    {
        while (!m_HandRotate)
        {
            Debug.Log("WireBlinkWait");

            if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
            {
                Debug.Log("キャンセル");
                StopCoroutine(m_LeftWireBlinkWaitCoroutine);
            }

            yield return null;
        }

        while (!OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            Debug.Log("トリガーが離されるのを待っている");

            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                Debug.Log("キャンセル");
                StopCoroutine(m_LeftWireBlinkWaitCoroutine);
            }

            yield return null;
        }

        m_PlayerAction.JointConnectedBodyRelease();

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = direction * m_PlayerInfo.GetBlinkPower();

        while (!m_BlinkEnd)
        {
            if (Vector3.Distance(m_BlinkStartPosition, transform.position) > m_PlayerInfo.GetBlinkInertiaPower())
            {
                m_BlinkEnd = true;
            }
            yield return null;
        }

        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(direction * m_PlayerInfo.GetBlinkInertiaPower(), ForceMode.Impulse);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == TagName.RESTART_POINT || other.tag == TagName.GOAL_POINT)
            return;

        m_BlinkEnd = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        m_BlinkEnd = true;
    }
}
