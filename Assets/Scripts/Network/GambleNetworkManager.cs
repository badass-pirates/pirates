using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GambleNetworkManager : NetworkManager
{
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
