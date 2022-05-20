using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ReadyNetworkManager : NetworkManager
{
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (actorNumbers == null)
        {
            InitActorNumbers();
        }
        if (PhotonNetwork.IsMasterClient)
        {
            InsertActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
            canSpawn = true;
        }
        StartCoroutine(SpawnPlayer());
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        int actorNumber = otherPlayer.ActorNumber;
        InsertActorNumber(actorNumber);
        MasterSendActorNumbers();
    }

    protected override void InitActorNumbers()
    {
        actorNumbers = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    }

    protected override (string, string) GetResourceName ()
    {
        string localPlayer = "Ready/Local Player";
        string networkPlayer = "Ready/Network Player";
        return (localPlayer, networkPlayer);
    }
}
