using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Utils;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Transform readyItemSpawnPoint;
    public float timer;
    private GameObject readyItem;


    #region Singleton
    public static ReadyManager RM;
    void Awake()
    {
        if (RM == null)
        {
            RM = this;
        }
        else Destroy(this);
    }
    #endregion

    public void ChangeScene()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Transform transform = readyItem.transform;
        PhotonNetwork.Destroy(readyItem);
        PhotonNetwork.Instantiate("Ready/SkullExplosion", transform.position, transform.rotation);

        StartCoroutine(U.ChangeScene("GambleScene", 1));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!PV.IsMine)
        {
            return;
        }
        readyItem = PhotonNetwork.Instantiate("Ready/Skull", readyItemSpawnPoint.position, readyItemSpawnPoint.rotation);
        ReadyItem item = readyItem.GetComponent<ReadyItem>();
        item.SetTimer(timer);
    }
}
