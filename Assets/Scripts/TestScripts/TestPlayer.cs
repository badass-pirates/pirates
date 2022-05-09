using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestPlayer : MonoBehaviourPun, IPunObservable
{
    PhotonView PV;
    Vector3 curPos;

    // Start is called before the first frame update
    void Start()
    {
        PV = photonView;
        if (!PV.IsMine) return;
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }

    // Update is called once per frame
    void Update()
    {

        if (!PV.IsMine) { 
            OtherMove();
            return;
        }

        Move();
    }

    void OtherMove()
    {
        if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(transform.position);
        else
            curPos = (Vector3)stream.ReceiveNext();
    }
}
