using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject gunFx, bloodFx, dustFx;
    RaycastHit hit;
    bool is_Hit = false;


    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }

    void DrawLine()
    {
        Vector3 direction = transform.forward;
        is_Hit = Physics.Raycast(transform.position, direction, out hit);
        if (is_Hit)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);

        }
    }

    public void Shoot()
    {
        int actorNumber = -1;

        if (!is_Hit) return;
        Debug.Log("Gun Hit "+hit.transform.name);
        //#9 is body layer
        if (hit.transform.gameObject.layer == 9)
        {
            PhotonView pv = hit.transform.GetComponent<PhotonView>();
            if (pv == null) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            else actorNumber = pv.ViewID / 1000;
        }
        PlayHitEffect(actorNumber, hit.transform);
    }

    void PlayHitEffect(int actorNumber, Transform tr)
    {
        Quaternion rotateValue = Quaternion.Euler(new Vector3(0, 180, 0));
        if (actorNumber == -1)
            PhotonNetwork.Instantiate(dustFx.name, tr.position, tr.rotation*rotateValue);
        else
            PhotonNetwork.Instantiate(bloodFx.name, tr.position, tr.rotation*rotateValue);
    }
}
