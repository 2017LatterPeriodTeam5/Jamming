using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_PlayerWireAction1024 : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform m_Trans;
    private Rigidbody m_Rigid;
    private RightHandPull m_RightPull;
    private LeftHandPull m_LeftPull;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("力の強さ")]
    private float m_Power = 3.0f;
    [SerializeField, TooltipAttribute("レイの長さ")]
    private float distance = 1.0f;
    [SerializeField, TooltipAttribute("移動量")]
    private float m_AxisSpeed = 5.0f;

    /*==内部設定変数==*/
    //カメラのトランスフォーム
    private Transform m_Camera;

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

    //右手のAnchorのトランスフォーム
    public Transform m_RightHand;
    //左のAnchorのトランスフォーム
    public Transform m_LeftHand;

    private bool m_RightForceFlag = false;
    private bool m_LeftForceFlag = false;

    private enum HandType
    {
        None,
        Right,
        Left
    }
    private HandType m_HandType = HandType.None;

    public void Awake()
    {
        //コンポーネント取得
        m_Trans = GetComponent<Transform>();
        m_Rigid = GetComponent<Rigidbody>();
        m_RightPull = GetComponent<RightHandPull>();
        m_LeftPull = GetComponent<LeftHandPull>();

        //カーソルを隠す・ロックする
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag(TagName.MAIN_CAMERA).GetComponent<Transform>();

        m_RightBasePoint = GameObject.Find("RightBasePoint").GetComponent<Transform>();
        m_RightJoint = m_RightBasePoint.GetComponent<SpringJoint>();
        m_RightLine = m_RightBasePoint.GetComponent<LineRenderer>();

        m_LeftBasePoint = GameObject.Find("LeftBasePoint").GetComponent<Transform>();
        m_LeftJoint = m_LeftBasePoint.GetComponent<SpringJoint>();
        m_LeftLine = m_LeftBasePoint.GetComponent<LineRenderer>();
    }

    void Update()
    {
        //カーソルを表示する・ロックを解除する
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        RightHandAction();

        LeftHandAction();

        StopTakeUp();
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
            }

            m_RightJoint.maxDistance = Vector3.Distance(m_RightHand.position, m_RightBasePoint.position);
            m_RightJoint.minDistance = Vector3.Distance(m_RightHand.position, m_RightBasePoint.position);

            Vector3 moveDirection = m_RightBasePoint.position - m_Trans.position;
            m_Rigid.velocity = moveDirection.normalized * m_Power;
        }

        if (m_RightPull.GetTouchPull() == true && m_RightJoint.connectedBody != null)
        {
            //velocityをリセット
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = false;
            //m_RightJoint.connectedBody = null;

            //m_RightForceFlag = true;
            m_HandType = HandType.Right;
        }

        //if (m_RightJoint.connectedBody == null && m_HandType == HandType.Right)
        //{
        //    float dis = Vector3.Distance(m_Trans.position, m_RightBasePoint.position);

        //    if (dis <= 5.0f)
        //    {
        //        m_RightJoint.maxDistance = 1.0f;
        //        m_RightJoint.minDistance = 1.0f;
        //        m_RightJoint.connectedBody = m_Rigid;
        //        m_Rigid.useGravity = true;
        //        m_ForceFlag = false;
        //    }
        //}

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            RaycastHit hitInto;
            Ray ray = new Ray(m_RightHand.position, m_RightHand.forward);
            Physics.Raycast(ray, out hitInto, Mathf.Infinity);

            if (hitInto.collider == null) return;

            //m_HandType = HandType.Right;
            JointConnectedBodyRelease();

            m_RightJoint.maxDistance = hitInto.distance - 2.0f;
            m_RightJoint.minDistance = hitInto.distance - 2.0f;

            m_RightBasePoint.position = hitInto.point;

            m_RightJoint.connectedBody = m_Rigid;

            m_RightLine.enabled = true;
            m_LeftLine.enabled = false;

            m_Rigid.useGravity = true;

            ForceFlagRelease();
        }

        m_RightLine.SetPosition(0, m_RightHand.position);
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
            }

            m_LeftJoint.maxDistance = Vector3.Distance(m_LeftHand.position, m_LeftBasePoint.position);
            m_LeftJoint.minDistance = Vector3.Distance(m_LeftHand.position, m_LeftBasePoint.position);

            Vector3 moveDirection = m_LeftBasePoint.position - m_Trans.position;
            m_Rigid.velocity = moveDirection.normalized * m_Power;
        }

        if (m_LeftPull.GetTouchPull() == true && m_LeftJoint.connectedBody != null)
        {
            //velocityをリセット
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = false;
            //m_LeftJoint.connectedBody = null;

            Vector3 moveDirection = m_LeftBasePoint.position - m_Trans.position;
            m_Rigid.velocity = moveDirection.normalized * m_Power;

            //m_LeftForceFlag = true;
            m_HandType = HandType.Left;
        }

        //if (m_LeftJoint.connectedBody == null && m_HandType == HandType.Left)
        //{
        //    float dis = Vector3.Distance(m_Trans.position, m_LeftBasePoint.position);

        //    if (dis <= 5.0f)
        //    {
        //        m_LeftJoint.maxDistance = 1.0f;
        //        m_LeftJoint.minDistance = 1.0f;
        //        m_LeftJoint.connectedBody = m_Rigid;
        //        m_Rigid.useGravity = true;
        //        m_ForceFlag = false;
        //    }
        //}

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            RaycastHit hitInto;
            Ray ray = new Ray(m_LeftHand.position, m_LeftHand.forward);
            Physics.Raycast(ray, out hitInto, Mathf.Infinity);

            if (hitInto.collider == null) return;

            //m_HandType = HandType.Left;
            JointConnectedBodyRelease();

            m_LeftJoint.maxDistance = hitInto.distance - 2.0f;
            m_LeftJoint.minDistance = hitInto.distance - 2.0f;

            m_LeftBasePoint.position = hitInto.point;

            m_LeftJoint.connectedBody = m_Rigid;

            m_LeftLine.enabled = true;
            m_RightLine.enabled = false;

            m_Rigid.useGravity = true;

            ForceFlagRelease();
        }

        m_LeftLine.SetPosition(0, m_LeftHand.position);
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

    private void JointConnectedBodyRelease()
    {
        m_RightJoint.connectedBody = null;
        m_LeftJoint.connectedBody = null;
    }

    private void ForceFlagRelease()
    {
        m_RightForceFlag = false;
        m_LeftForceFlag = false;
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (m_ForceFlag == true)
    //    {
    //        m_Rigid.velocity = Vector3.zero;
    //        m_Rigid.angularVelocity = Vector3.zero;

    //        m_Rigid.useGravity = true;
    //        JointConnectedBodyRelease();

    //        m_ForceFlag = false;

    //        m_RightLine.enabled = false;
    //        m_LeftLine.enabled = false;
    //    }
    //}
}
