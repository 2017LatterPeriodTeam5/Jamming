using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_PlayerWireAction : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform m_Trans;
    private Rigidbody m_Rigid;
    private RightHandPull m_RightPull;
    private LeftHandPull m_LeftPull;
    private WireBlink m_WireBlink;
    private PlayerInfo m_PlayerInfo;

    /*==内部設定変数==*/
    //カメラのトランスフォーム
    private Transform m_Camera;
    //移動の基点
    private GameObject m_MoveBase;

    //右手の基点のトランスフォーム
    private Transform m_RightBasePoint;
    //右手の基点のSpringJoint
    private SpringJoint m_RightJoint;
    //右手のLineRenderer
    private LineRenderer m_RightLine;

    //左手の基点のトランスフォーム
    private Transform m_LeftBasePoint;
    //左手の基点のSpringJoint
    private SpringJoint m_LeftJoint;
    //左手のLineRenderer
    private LineRenderer m_LeftLine;

    //地面に接しているかどうか
    private bool m_IsGround = false;
    //移動スティックのX軸の値
    private float Axis_X = 0;
    //移動スティックのY軸の値
    private float Axis_Y = 0;
    //プレイヤー以外と当たるLayerMask
    private int m_NotPlayerLayerMask;

    public enum HandType
    {
        None,
        Right,
        Left
    }
    [SerializeField, TooltipAttribute("地面との判定の長さ(基本変えない)")]
    private HandType m_HandType = HandType.None;

    public void Awake()
    {
        //コンポーネント取得
        m_Trans = GetComponent<Transform>();
        m_Rigid = GetComponent<Rigidbody>();
        m_RightPull = GetComponent<RightHandPull>();
        m_LeftPull = GetComponent<LeftHandPull>();
        m_WireBlink = GetComponent<WireBlink>();
        m_PlayerInfo = GetComponent<PlayerInfo>();

        //カーソルを隠す・ロックする
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag(TagName.MAIN_CAMERA).GetComponent<Transform>();
        m_MoveBase = GameObject.Find("PlayerMoveBase");

        m_RightBasePoint = GameObject.Find("RightBasePoint").GetComponent<Transform>();
        m_RightJoint = m_RightBasePoint.GetComponent<SpringJoint>();
        m_RightLine = m_RightBasePoint.GetComponent<LineRenderer>();

        m_LeftBasePoint = GameObject.Find("LeftBasePoint").GetComponent<Transform>();
        m_LeftJoint = m_LeftBasePoint.GetComponent<SpringJoint>();
        m_LeftLine = m_LeftBasePoint.GetComponent<LineRenderer>();

        m_NotPlayerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    void Update()
    {
        //カーソルを表示する・ロックを解除する
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        Move();

        RightHandAction();

        LeftHandAction();

        StopTakeUp();
    }

    /**==============================================================================================*/
    /** Update内で実行
    /**==============================================================================================*/

    private void Move()
    {
        RaycastHit hitDown;
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hitDown, m_PlayerInfo.GetGroundCheckDistance()))
        {
            m_IsGround = true;
        }
        else
        {
            m_IsGround = false;
        }

        if (OVRInput.Get(OVRInput.RawButton.LThumbstickUp))
        {
            Axis_Y = 1.0f;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LThumbstickDown))
        {
            Axis_Y = -1.0f;
        }
        else
        {
            Axis_Y = 0.0f;
        }

        if (OVRInput.Get(OVRInput.RawButton.LThumbstickLeft))
        {
            Axis_X = -1.0f;
        }
        else if (OVRInput.Get(OVRInput.RawButton.LThumbstickRight))
        {
            Axis_X = 1.0f;
        }
        else
        {
            Axis_X = 0.0f;
        }

        m_MoveBase.transform.forward = new Vector3(m_Camera.forward.x, m_Trans.forward.y, m_Camera.forward.z);

        if (m_IsGround == true)
        {
            transform.position += m_MoveBase.transform.forward * Axis_Y * m_PlayerInfo.GetMoveSpeed() * Time.deltaTime
                               + m_MoveBase.transform.right * Axis_X * m_PlayerInfo.GetMoveSpeed() * Time.deltaTime;
        }
    }

    private void RightHandAction()
    {
        if (m_HandType == HandType.Right)
        {
            float dis = Vector3.Distance(m_Trans.position, m_RightBasePoint.position);
            if (dis < 1.5f)
            {
                m_HandType = HandType.None;
                m_Rigid.useGravity = true;
                return;
            }

            m_RightJoint.maxDistance = Vector3.Distance(m_PlayerInfo.GetRightHand().position, m_RightBasePoint.position);
            m_RightJoint.minDistance = Vector3.Distance(m_PlayerInfo.GetRightHand().position, m_RightBasePoint.position);

            Vector3 moveDirection = m_RightBasePoint.position - m_PlayerInfo.GetRightHand().position;
            m_Rigid.velocity = moveDirection.normalized * m_PlayerInfo.GetPullPower();

            //float jointDis = Vector3.Distance(m_PlayerInfo.GetRightHand().position, m_RightBasePoint.position);
            //m_RightJoint.maxDistance = jointDis;
            //m_RightJoint.minDistance = jointDis;
            //m_RightJoint.maxDistance = Vector3.Distance(m_PlayerInfo.GetRightHand().position, m_RightBasePoint.position);
            //m_RightJoint.minDistance = Vector3.Distance(m_PlayerInfo.GetRightHand().position, m_RightBasePoint.position);

            //Vector3 moveDirection = m_RightBasePoint.position - m_PlayerInfo.GetRightHand().position;
            //m_Rigid.velocity = moveDirection.normalized * m_PlayerInfo.GetPullPower();

            //float jointDis = Vector3.Distance(m_Trans.position, m_RightBasePoint.position);
            //m_RightJoint.maxDistance = jointDis;
            //m_RightJoint.minDistance = jointDis;

            //Vector3 moveDirection = m_RightBasePoint.position - m_PlayerInfo.GetRightHand().position;
            //m_Rigid.velocity = moveDirection.normalized * m_PlayerInfo.GetPullPower();

        }

        if (m_RightPull.GetTouchPull() == true && m_RightJoint.connectedBody != null)
        {
            //velocityをリセット
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = false;

            m_HandType = HandType.Right;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            RaycastHit hitInto;
            Ray ray = new Ray(m_PlayerInfo.GetRightHand().position, m_PlayerInfo.GetRightHand().forward);
            Physics.Raycast(ray, out hitInto, m_PlayerInfo.GetWireDistance(), m_NotPlayerLayerMask, QueryTriggerInteraction.Ignore);

            if (hitInto.collider == null)
            {
                m_RightLine.enabled = false;
                m_LeftLine.enabled = false;
                m_WireBlink.WireBlinkWaitStart(m_PlayerInfo.GetRightHand(), ray.direction, HandType.Right);
            }
            else
            {
                JointConnectedBodyRelease();

                m_RightJoint.maxDistance = hitInto.distance - 2.0f;
                m_RightJoint.minDistance = hitInto.distance - 2.0f;

                m_RightBasePoint.position = hitInto.point;

                m_RightJoint.connectedBody = m_Rigid;

                m_RightLine.enabled = true;
                m_LeftLine.enabled = false;

                m_Rigid.useGravity = true;
            }
        }

        if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            JointConnectedBodyRelease(HandType.Right);
            m_RightLine.enabled = false;

            m_Rigid.useGravity = true;
        }

        m_RightLine.SetPosition(0, m_PlayerInfo.GetRightHand().position);
        m_RightLine.SetPosition(1, m_RightBasePoint.position);
    }

    private void LeftHandAction()
    {
        if (m_HandType == HandType.Left)
        {
            float dis = Vector3.Distance(m_Trans.position, m_LeftBasePoint.position);
            if (dis < 1.5f)
            {
                m_HandType = HandType.None;
                m_Rigid.useGravity = true;
                return;
            }

            m_LeftJoint.maxDistance = Vector3.Distance(m_PlayerInfo.GetLeftHand().position, m_LeftBasePoint.position);
            m_LeftJoint.minDistance = Vector3.Distance(m_PlayerInfo.GetLeftHand().position, m_LeftBasePoint.position);

            Vector3 moveDirection = m_LeftBasePoint.position - m_PlayerInfo.GetLeftHand().position;
            m_Rigid.velocity = moveDirection.normalized * m_PlayerInfo.GetPullPower();
        }

        if (m_LeftPull.GetTouchPull() == true && m_LeftJoint.connectedBody != null)
        {
            //velocityをリセット
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = false;

            m_HandType = HandType.Left;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            RaycastHit hitInto;
            Ray ray = new Ray(m_PlayerInfo.GetLeftHand().position, m_PlayerInfo.GetLeftHand().forward);
            Physics.Raycast(ray, out hitInto, m_PlayerInfo.GetWireDistance(), m_NotPlayerLayerMask, QueryTriggerInteraction.Ignore);

            if (hitInto.collider == null)
            {
                m_RightLine.enabled = false;
                m_LeftLine.enabled = false;
                m_WireBlink.WireBlinkWaitStart(m_PlayerInfo.GetLeftHand(), ray.direction, HandType.Left);
            }
            else
            {
                JointConnectedBodyRelease();

                m_LeftJoint.maxDistance = hitInto.distance - 2.0f;
                m_LeftJoint.minDistance = hitInto.distance - 2.0f;

                m_LeftBasePoint.position = hitInto.point;

                m_LeftJoint.connectedBody = m_Rigid;

                m_LeftLine.enabled = true;
                m_RightLine.enabled = false;

                m_Rigid.useGravity = true;
            }
        }

        if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            JointConnectedBodyRelease(HandType.Left);
            m_LeftLine.enabled = false;

            m_Rigid.useGravity = true;
        }

        m_LeftLine.SetPosition(0, m_PlayerInfo.GetLeftHand().position);
        m_LeftLine.SetPosition(1, m_LeftBasePoint.position);
    }

    private void StopTakeUp()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A)
            || OVRInput.GetDown(OVRInput.RawButton.X))
        {
            m_HandType = HandType.None;
            m_Rigid.useGravity = true;
        }
    }

    /**==============================================================================================*/
    /** 上記関数内で使用
    /**==============================================================================================*/

    /**==============================================================================================*/
    /** 外部から使用する
    /**==============================================================================================*/

    /// <summary>
    /// 手のJointのConnectedBodyを解除する
    /// </summary>
    /// <param name="handType">どちらの手か？ デフォルトは両手</param>
    public void JointConnectedBodyRelease(HandType handType = HandType.None)
    {
        if (handType == HandType.None)
        {
            m_RightJoint.connectedBody = null;
            m_LeftJoint.connectedBody = null;
        }
        else if (handType == HandType.Right)
        {
            m_RightJoint.connectedBody = null;
        }
        else if (handType == HandType.Left)
        {
            m_LeftJoint.connectedBody = null;
        }
    }

    /// <summary>
    /// 両方のLineRendererの表示を消す
    /// </summary>
    public void WireLineDelete()
    {
        m_RightLine.enabled = false;
        m_LeftLine.enabled = false;
    }
}
