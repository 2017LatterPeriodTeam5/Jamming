using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class MouseCameraController : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform m_Trans;
    private Rigidbody m_Rigid;
    private TouchAcceleration m_TouchAcc;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("X軸視点移動感度")]
    private float m_XAxisSensitivity = 40.0f;
    [SerializeField, TooltipAttribute("Y軸視点移動感度")]
    private float m_YAxisSensitivity = 40.0f;
    [SerializeField, TooltipAttribute("力の強さ")]
    private float m_Power = 3.0f;
    [SerializeField, TooltipAttribute("レイの長さ")]
    private float distance = 1.0f;
    [SerializeField, TooltipAttribute("移動量")]
    private float m_AxisSpeed = 5.0f;

    /*==内部設定変数==*/
    //カメラのトランスフォーム
    private Transform m_Camera;
    //基点1
    private Transform m_BasePoint1;
    //基点1のSpringJoint
    private SpringJoint m_Joint1;
    //基点2
    private Transform m_BasePoint2;
    //基点2のSpringJoint
    private SpringJoint m_Joint2;

    private float Axis_Y = 0;
    private float Axis_X = 0;

    private LineRenderer m_Line1;
    public Transform m_RightHand;
    private bool m_ForceFlag = false;
    private bool isGround = false;


    public void Awake()
    {
        //コンポーネント取得
        m_Trans = GetComponent<Transform>();
        m_Rigid = GetComponent<Rigidbody>();
        m_TouchAcc = GetComponent<TouchAcceleration>();

        //カーソルを隠す・ロックする
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag(TagName.MAIN_CAMERA).GetComponent<Transform>();

        m_BasePoint1 = GameObject.Find("RightBasePoint").GetComponent<Transform>();
        m_Joint1 = m_BasePoint1.GetComponent<SpringJoint>();
        m_Line1 = m_BasePoint1.GetComponent<LineRenderer>();

        m_BasePoint2 = GameObject.Find("LeftBasePoint").GetComponent<Transform>();
        m_Joint2 = m_BasePoint2.GetComponent<SpringJoint>();
    }

    public void FixedUpdate()
    {

    }

    void Update()
    {
        RaycastHit hit_Down;
        if (Physics.Raycast(transform.position, Vector3.down, out hit_Down, distance))
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }


        float acc = m_TouchAcc.GetTouchAcceleration();
        if ((Input.GetKeyDown(KeyCode.Z) || m_TouchAcc.GetTouchPull() == true) && m_Joint1.connectedBody != null)
        {
            // print(acc);

            //velocityをリセット
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = false;
            m_Joint1.connectedBody = null;

            //RaycastHit hitInto;
            //Ray ray = new Ray(tr.position, m_Camera.forward);
            //Physics.Raycast(ray, out hitInto, Mathf.Infinity);

            //if (hitInto.collider == null) return;

            //Vector3 moveDirection = hitInto.point - tr.position;

            //rg.AddForce(moveDirection.normalized * m_Power, ForceMode.Impulse);

            Vector3 moveDirection = m_BasePoint1.position - m_Trans.position;
            m_Rigid.velocity = moveDirection.normalized * m_Power;

            m_ForceFlag = true;
        }

        if (m_Joint1.connectedBody == null)
        {
            float dis = Vector3.Distance(m_Trans.position, m_BasePoint1.position);

            if (dis <= 5.0f)
            {
                m_Joint1.maxDistance = 1.0f;
                m_Joint1.minDistance = 1.0f;
                m_Joint1.connectedBody = m_Rigid;
                m_Rigid.useGravity = true;
                m_ForceFlag = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.X) || OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            RaycastHit hitInto;
            Ray ray = new Ray(m_RightHand.position, m_RightHand.forward);
            Physics.Raycast(ray, out hitInto, Mathf.Infinity);

            if (hitInto.collider == null) return;

            m_Joint1.maxDistance = hitInto.distance - 2.0f;
            m_Joint1.minDistance = hitInto.distance - 2.0f;

            m_BasePoint1.position = hitInto.point;

            m_Joint1.connectedBody = m_Rigid;

            m_Line1.enabled = true;

            m_Rigid.useGravity = true;
            m_ForceFlag = false;

            //rg.AddForce(m_Camera.forward * m_Power, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            m_Joint1.connectedBody = null;
            m_Joint2.connectedBody = null;

            m_Line1.enabled = false;
        }






        //if (OVRInput.Get(OVRInput.RawButton.LThumbstickUp))
        //{
        //    Axis_Y = 1.0f;
        //}
        //else if (OVRInput.Get(OVRInput.RawButton.LThumbstickDown))
        //{
        //    Axis_Y = -1.0f;
        //}
        //else
        //{
        //    Axis_Y = 0.0f;
        //}

        //if (OVRInput.Get(OVRInput.RawButton.LThumbstickLeft))
        //{
        //    Axis_X = -1.0f;
        //}
        //else if (OVRInput.Get(OVRInput.RawButton.LThumbstickRight))
        //{
        //    Axis_X = 1.0f;
        //}
        //else
        //{
        //    Axis_X = 0.0f;
        //}

        //if (isGround == true)
        //{
        //    transform.position += transform.forward * Axis_Y * m_AxisSpeed * Time.deltaTime
        //                       + transform.right * Axis_X * m_AxisSpeed * Time.deltaTime;
        //}

        m_Trans.Rotate(Vector3.up, Input.GetAxis("Mouse X") * m_XAxisSensitivity * Time.deltaTime);
        m_Camera.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * m_YAxisSensitivity * Time.deltaTime);

        //m_Line1.SetPosition(0, m_Trans.position + m_Trans.forward * 2.0f);
        m_Line1.SetPosition(0, m_RightHand.position);
        m_Line1.SetPosition(1, m_BasePoint1.position);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (m_ForceFlag == true)
        {
            m_Rigid.velocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_Rigid.useGravity = true;
            m_Joint1.connectedBody = null;

            m_ForceFlag = false;

            m_Line1.enabled = false;
        }
    }
}
