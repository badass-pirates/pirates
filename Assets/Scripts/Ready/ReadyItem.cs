using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static ReadyManager;
using Photon.Pun;
using UnityEngine.UI;


public class ReadyItem : XRGrabInteractable
{
    public float timer;
    public GameObject skullCanvas;
    public Text countText;

    private bool isGrabbed = false;
    private float leftTime = 5;
    private PhotonView photonView;

    void Start()
    {
        //if (PhotonNetwork.IsMasterClient) return;
        //enabled = false;
        photonView = gameObject.GetComponent<PhotonView>();

    }

    void Update()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        if (!isGrabbed) return;
        if (skullCanvas.activeSelf == true) countText.text = ((int)leftTime).ToString();
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
        photonView.RequestOwnership();
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
