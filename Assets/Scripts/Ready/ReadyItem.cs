using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static ReadyManager;
using Photon.Pun;

public class ReadyItem : XRGrabInteractable
{
    public float timer;

    private bool isGrabbed = false;
    private float leftTime;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) return;
        enabled = false;
    }

    void Update()
    {
        if (!isGrabbed) return;
        if (leftTime < 0)
        {
            RM.ChangeScene();
            return;
        }
        leftTime -= Time.deltaTime;
    }

    public void SetTimer(float _timer)
    {
        timer = _timer;
    }

    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);
        isGrabbed = true;
        leftTime = timer;
    }

    [System.Obsolete]
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        base.OnSelectExiting(interactor);
        isGrabbed = false;
        leftTime = timer;
    }
}
