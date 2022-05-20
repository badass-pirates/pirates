using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GambleNetworkManager : NetworkManager
{
    private void Start()
    {
        InitActorNumbers();
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                InsertActorNumber(player.ActorNumber);
            }
            canSpawn = true;
            MasterSendActorNumbers();
        }
        StartCoroutine(SpawnPlayer());
    }

    protected override void InitActorNumbers()
    {
        int size = PhotonNetwork.CurrentRoom.PlayerCount;
        actorNumbers = new int[size];
        for (int i = 0; i < actorNumbers.Length; i++)
        {
            actorNumbers[i] = -1;
        }
    }

    protected override (string, string) GetResourceName()
    {
        string localPlayer = "Local Player";
        string networkPlayer = "Network Player";
        return (localPlayer, networkPlayer);
    }
}
