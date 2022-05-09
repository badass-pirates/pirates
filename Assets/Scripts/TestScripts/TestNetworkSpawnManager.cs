using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestNetworkSpawnManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    private GameObject spawnedPlayer;
    private int[] playerInfos = { -1, -1, -1, -1, -1, -1, -1, -1 };
    private bool canStart = false;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            InsertActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
            canStart = true;
        }
        StartCoroutine(SpawnPlayer());
    }

    void InsertActorNumber(int actorNumber)
    {
        int index = Array.FindIndex<int>(playerInfos, x => x == -1);
        playerInfos[index] = actorNumber;
    }

    IEnumerator SpawnPlayer()
    {
        while (!canStart) yield return null;

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = Array.FindIndex<int>(playerInfos, x => x == actorNumber);
        Transform startPoint = CalculateStartPoint(index);
        spawnedPlayer = PhotonNetwork.Instantiate("Test Player", startPoint.position, startPoint.rotation);
    }

    public Transform CalculateStartPoint(int index)
    {
        GameObject empty = new GameObject();
        Transform startPoint = empty.transform;
        startPoint.Rotate(new Vector3(0, 360 / 8 * index, 0));
        startPoint.Translate(new Vector3(0, 0, -2));
        return startPoint;
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        int actorNumber = otherPlayer.ActorNumber;
        InsertActorNumber(actorNumber);
        MasterSendPlayerInfo();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            MasterRemovePlayerInfo(otherPlayer.ActorNumber);
        }
    }

    void MasterRemovePlayerInfo(int actorNum)
    {
        // OnPlayerLeftRoom으로 방을 나갈경우 플레이어 제거
        int index = Array.FindIndex<int>(playerInfos, x => x == actorNum);
        playerInfos[index] = -1;
        MasterSendPlayerInfo();
    }

    void MasterSendPlayerInfo()
    {
        PV.RPC("RPC_OtherReceivePlayerInfo", RpcTarget.Others, playerInfos);
    }

    [PunRPC]
    void RPC_OtherReceivePlayerInfo(int[] _playerInfos)
    {
        canStart = true;
        playerInfos = _playerInfos;
    }
}
