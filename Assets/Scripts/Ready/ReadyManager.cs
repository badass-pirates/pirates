using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Transform readyItemSpawnPoint;
    public float timer;


    #region Singleton
    public static ReadyManager RM;
    void Awake()
    {
        if (RM == null)
        {
            RM = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }
    #endregion

    public void ChangeScene()
    {
        PhotonView[] PVs = GameObject.FindObjectsOfType<PhotonView>();
        foreach(PhotonView pv in PVs)
        {
            Destroy(pv);
        }
        PhotonNetwork.LoadLevel("GambleScene");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!PV.IsMine)
        {
            return;
        }
        GameObject item = PhotonNetwork.Instantiate("Ready/Skull", readyItemSpawnPoint.position, readyItemSpawnPoint.rotation);
        ReadyItem readyItem = item.GetComponent<ReadyItem>();
        readyItem.SetTimer(timer);
    }
}
