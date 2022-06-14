using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform barrel;
    public GameObject bloodFx, dustFx;
    RaycastHit hit;
    bool is_Hit = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void DrawLine()
    {
        Vector3 direction = barrel.transform.forward;
        is_Hit = Physics.Raycast(barrel.transform.position, direction, out hit);
        if (is_Hit)
            lineRenderer.SetPosition(1, hit.point);
    }

    int Shoot()
    {
        int actorNumber = -1;

        if (!is_Hit) return actorNumber;

        //#9 is body layer
        if (hit.transform.gameObject.layer == 9)
        {
            PhotonView pv = hit.transform.GetComponent<PhotonView>();
            if (pv == null) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            else actorNumber = pv.ViewID / 1000;
        }

        return actorNumber;
    }

    void PlayHitEffect(int actorNumber, Vector3 pos)
    {
        //if (actorNumber == -1)
    }
}
