using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class TrackerAnchorSetting : MonoBehaviour
{
    public Transform player;

    public Transform[] anchors;
    public GameObject VRcamera;

    void Start()
    {
        VRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
    }

    void Update()
    {

        Vector3 basePos = player.position;

        Vector3 trackingPos = InputTracking.GetLocalPosition(VRNode.CenterEye);

        //VRcamera.transform.position = basePos - trackingPos;

        //位置トラッキングリセット
        if (Input.GetKeyDown(KeyCode.Return))
        {
            InputTracking.Recenter();
        }
    }

    void LateUpdate()
    {
        //foreach (Transform tr in anchors)
        //{
        //    tr.position = player.position;
        //}

    }
}
