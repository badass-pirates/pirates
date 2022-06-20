using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

[System.Serializable]
public class VRNetworkPlayerRig : MonoBehaviourPunCallbacks
{
    private Transform vrHeadModel;
    public Transform rigHeadModel;
    public VRMap head;
    public PhotonView PV;
    public GameObject vrPlayer;
    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public float turnSmoothness = 5f;

    private void Start()
    {
        if (PV.IsMine)
        {
            headBodyOffset = transform.position - headConstraint.position;
            head.vrTarget = GameObject.Find("Player").transform.Find("Camera Offset/Main Camera");
            vrHeadModel = GameObject.Find("Player").transform.Find("Camera Offset/Model/Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head");
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            vrPlayer.SetActive(false);
            transform.position = headConstraint.position + headBodyOffset;
            transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(-headConstraint.up, -Vector3.up).normalized, Time.deltaTime * turnSmoothness);
            head.Map();
            rigHeadModel.localScale = vrHeadModel.localScale;
        }
    }
}