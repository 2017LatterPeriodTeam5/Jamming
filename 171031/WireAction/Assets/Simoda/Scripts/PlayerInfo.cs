using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("地面との判定の長さ(基本変えない)")]
    private float m_GroundCheckDistance = 1.5f;
    [SerializeField, TooltipAttribute("移動スピード")]
    private float m_MoveSpeed = 5.0f;
    [SerializeField, TooltipAttribute("ワイヤーの長さ")]
    private float m_WireDistance = 30.0f;
    [SerializeField, TooltipAttribute("張り付きで移動するときの強さ")]
    private float m_PullPower = 80.0f;
    [SerializeField, TooltipAttribute("引いたと判定するときに必要な過去の手と現在の手の距離")]
    private float m_PullDistance = 1.5f;
    [SerializeField, TooltipAttribute("引いたかどうかを計算する周期")]
    private float m_PullTime = 0.3f;
    [SerializeField, TooltipAttribute("捻ったと判定するときに必要な角度")]
    private float m_TwistAngle = 100.0f;
    [SerializeField, TooltipAttribute("捻ったかどうかを計算する周期")]
    private float m_TwistTime = 0.3f;
    [SerializeField, TooltipAttribute("ブリンクで移動するときの強さ")]
    private float m_BlinkPower = 150.0f;
    [SerializeField, TooltipAttribute("ブリンクが終わった後の慣性の強さ")]
    private float m_BlinkInertiaPower = 30.0f;
    [SerializeField, TooltipAttribute("ブリンク再使用までの時間")]
    private float m_BlinkRecastTime = 5.0f;

    /*==内部設定変数==*/
    //右手のトランスフォーム
    private Transform m_RightHand;
    //左手のトランスフォーム
    private Transform m_LeftHand;

    void Awake()
    {
        //オブジェクトの取得
        m_RightHand = GameObject.Find("RightHandAnchor").GetComponent<Transform>();
        m_LeftHand = GameObject.Find("LeftHandAnchor").GetComponent<Transform>();
    }

    void Start ()
    {

    }
	
	void Update ()
    {
		
	}

    /**==============================================================================================*/
    /** 外部から使用する
    /**==============================================================================================*/

    /// <summary>
    /// 地面との判定の長さを返す
    /// </summary>
    /// <returns>地面との判定の長さ</returns>
    public float GetGroundCheckDistance()
    {
        return m_GroundCheckDistance;
    }

    /// <summary>
    /// 移動スピードを返す
    /// </summary>
    /// <returns>移動スピード</returns>
    public float GetMoveSpeed()
    {
        return m_MoveSpeed;
    }

    /// <summary>
    /// ワイヤーの長さを返す
    /// </summary>
    /// <returns>ワイヤーの長さ</returns>
    public float GetWireDistance()
    {
        return m_WireDistance;
    }

    /// <summary>
    /// 張り付きで移動するときの強さを返す
    /// </summary>
    /// <returns>張り付きで移動するときの強さ</returns>
    public float GetPullPower()
    {
        return m_PullPower;
    }

    /// <summary>
    /// 引いたと判定するときに必要な過去の手と現在の手の距離を返す
    /// </summary>
    /// <returns>引いたと判定するときに必要な過去の手と現在の手の距離</returns>
    public float GetPullDistance()
    {
        return m_PullDistance;
    }

    /// <summary>
    /// 引いたかどうかを計算する周期を返す
    /// </summary>
    /// <returns>引いたかどうかを計算する周期</returns>
    public float GetPullTime()
    {
        return m_PullTime;
    }

    /// <summary>
    /// 捻ったと判定するときに必要な角度を返す
    /// </summary>
    /// <returns>捻ったと判定するときに必要な角度</returns>
    public float GetTwistAngle()
    {
        return m_TwistAngle;
    }

    /// <summary>
    /// 捻ったかどうかを計算する周期を返す
    /// </summary>
    /// <returns>捻ったかどうかを計算する周期</returns>
    public float GetTwistTime()
    {
        return m_TwistTime;
    }

    /// <summary>
    /// ブリンクで移動するときの強さを返す
    /// </summary>
    /// <returns>ブリンクで移動するときの強さ</returns>
    public float GetBlinkPower()
    {
        return m_BlinkPower;
    }

    /// <summary>
    /// ブリンクが終わった後の慣性の強さを返す
    /// </summary>
    /// <returns>ブリンクが終わった後の慣性の強さ</returns>
    public float GetBlinkInertiaPower()
    {
        return m_BlinkInertiaPower;
    }

    /// <summary>
    /// ブリンク再使用までの時間を返す
    /// </summary>
    /// <returns>ブリンク再使用までの時間</returns>
    public float GetBlinkRecastTime()
    {
        return m_BlinkRecastTime;
    }

    /// <summary>
    /// 右手のトランスフォームを返す
    /// </summary>
    /// <returns>右手のトランスフォーム</returns>
    public Transform GetRightHand()
    {
        return m_RightHand;
    }

    /// <summary>
    /// 左手のトランスフォームを返す
    /// </summary>
    /// <returns>左手のトランスフォーム</returns>
    public Transform GetLeftHand()
    {
        return m_LeftHand;
    }
}
