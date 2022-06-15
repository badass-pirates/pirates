using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject gunFx, bloodFx, dustFx;
    
    public GameObject barrel, target;
    RaycastHit hit;
    bool isHit = false, isGrab = false;


    // Update is called once per frame
    void Update()
    {
        DrawLine();
    }

    public void CheckGrab(bool isGrab)
    {
        this.isGrab = isGrab;
        barrel.SetActive(isGrab);
    }

    void DrawLine()
    {
        Vector3 direction = barrel.transform.forward;
        isHit = Physics.Raycast(barrel.transform.position, direction, out hit);
        if (isHit)
        {
            lineRenderer.SetPosition(0, barrel.transform.position);
            lineRenderer.SetPosition(1, hit.point);
            target.transform.position = hit.point;
            if(hit.transform == transform) {isHit = false; return;}
        }
    }

    public void Shoot()
    {
        int actorNumber = -1;
        if(!isGrab) return;

        DrawLine();
        PhotonNetwork.Instantiate(gunFx.name, barrel.transform.position, barrel.transform.rotation);

        if (!isHit) return;
        //#9 is body layer
        if (hit.transform.gameObject.layer == 9)
        {
            PhotonView pv = hit.transform.GetComponent<PhotonView>();
            if (pv == null) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            else actorNumber = pv.ViewID / 1000;
        }
        PlayHitEffect(actorNumber, target.transform);
        Debug.Log("Gun Hit "+hit.transform.name+" #"+actorNumber);
        
    }

    void PlayHitEffect(int actorNumber, Transform tr)
    {
        if (actorNumber == -1)
            PhotonNetwork.Instantiate(dustFx.name, tr.position, tr.rotation);
        else
            PhotonNetwork.Instantiate(bloodFx.name, tr.position, tr.rotation);
    }
}
