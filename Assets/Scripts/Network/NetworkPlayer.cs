using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{

    public Transform leftHand;
    public Transform rightHand;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private PhotonView photonView;

    private Transform leftHandRig;
    private Transform rightHandRig;

    private GameObject localPlayer;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        if (photonView.IsMine)
        {
            localPlayer = Instantiate(Resources.Load<GameObject>("Local Player"), new Vector3(3f,3f,5f), Quaternion.identity);
            GambleManager.SetPlayer(localPlayer);
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }

        leftHandRig = GameObject.Find("Player").transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = GameObject.Find("Player").transform.Find("Camera Offset/RightHand Controller");
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            UpdateHandAnimator(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimator(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    void UpdateHandAnimator(InputDevice targetDevice, Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

}
