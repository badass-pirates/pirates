using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    public PhotonView photonView;
    public Rigidbody rig;

    private void Start()
    {
        if (photonView.IsMine) return;
        rig.useGravity = false;
        enabled = false;
    }

    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        photonView.RequestOwnership();
        photonView.RPC("SetUseGravity", RpcTarget.All, false);
        base.OnSelectEntered(interactor);
    }

    [System.Obsolete]
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        photonView.RPC("SetUseGravity", RpcTarget.All, true);
        base.OnSelectExiting(interactor);
    }

    [PunRPC]
    protected void SetUseGravity(bool value)
    {
        rig.useGravity = value;
    }
}
