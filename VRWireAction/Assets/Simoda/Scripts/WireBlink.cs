using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBlink : MonoBehaviour
{
    public Transform m_Hand;

    void Start()
    {

    }

    void Update()
    {
        //Debug.Log("x = " + Mathf.Floor(m_Hand.localRotation.eulerAngles.x) + " y = " +
        //    Mathf.Floor(m_Hand.localRotation.eulerAngles.y) + "  z = " +
        //    Mathf.Floor(m_Hand.localRotation.eulerAngles.z));
    }
}
