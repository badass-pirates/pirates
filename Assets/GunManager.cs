using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using UnityEngine.EventSystems;

public class GunManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject gunFx, bloodFx, dustFx;
    
    public GameObject barrel, aimingPoint;
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
            aimingPoint.transform.position = hit.point;
            if(hit.transform == transform) {isHit = false; return;}
        }
    }

    public void Shoot()
    {
        int targetActorNumber = -1;
        if(!isGrab) return;

        DrawLine();

        if (!isHit) return;
        //#9 is body layer
        GameObject target = hit.transform.gameObject;
        if (target.layer == 9)
        {
            PhotonView pv = target.GetComponent<PhotonView>();
            targetActorNumber = pv ? pv.ViewID / 1000 : -1;
        }
        if (target)
        {
            PhotonNetwork.Destroy(target);
        }
        PlayHitEffect(target.layer == 9, aimingPoint.transform);
        GambleManager.Attack(targetActorNumber);
        PhotonNetwork.Destroy(gameObject);
    }

    void PlayHitEffect(bool isPlayer, Transform tr)
    {
        PhotonNetwork.Instantiate(gunFx.name, barrel.transform.position, barrel.transform.rotation);
        PhotonNetwork.Instantiate(isPlayer ? bloodFx.name : dustFx.name, tr.position, tr.rotation);
    }
}
