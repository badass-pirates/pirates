using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    private GameObject spawnedPlayer;
    private int[] actorNumbers;
    private bool canSpawn = false;

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int size = sceneName.StartsWith("Gamble") ? PhotonNetwork.CurrentRoom.PlayerCount : 8;
        actorNumbers = new int[size];
        for (int i = 0; i < actorNumbers.Length; i++)
        {
            actorNumbers[i] = -1;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            InsertActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
            canSpawn = true;
        }
        StartCoroutine(SpawnPlayer());
    }

    void InsertActorNumber(int actorNumber)
    {
        int index = Array.FindIndex<int>(actorNumbers, x => x == -1);
        actorNumbers[index] = actorNumber;
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitUntil(() => canSpawn);

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = Array.FindIndex<int>(actorNumbers, x => x == actorNumber);
        Transform startPoint = CalculateStartPoint(index);
        if (PV.IsMine)
        {
            GameObject localPlayer = Instantiate(Resources.Load<GameObject>("Test Local Player"), startPoint.position, startPoint.rotation);
        }
        spawnedPlayer = PhotonNetwork.Instantiate("Test Network Player", startPoint.position, startPoint.rotation);
        yield break;
    }

    public Transform CalculateStartPoint(int index)
    {
        GameObject empty = new GameObject();
        Transform startPoint = empty.transform;
        startPoint.Translate(new Vector3(2, -2, 2)); // ������ �𸣰ڴµ� �߾��� ������
        startPoint.Rotate(new Vector3(0, 360 / actorNumbers.Length * index, 0)); // �ʿ��� ������ŭ ȸ��
        startPoint.Translate(new Vector3(0, 0, -4)); // ���̺� ���̸�ŭ �Ĺ����� �̵�
        return startPoint;
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            return;
        }
        int actorNumber = otherPlayer.ActorNumber;
        InsertActorNumber(actorNumber);
        MasterSendActorNumbers();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            return;
        }
        MasterRemovePlayerInfo(otherPlayer.ActorNumber);
    }

    void MasterRemovePlayerInfo(int actorNum)
    {
        // OnPlayerLeftRoom���� ���� ������� �÷��̾� ����
        int index = Array.FindIndex<int>(actorNumbers, x => x == actorNum);
        actorNumbers[index] = -1;
        MasterSendActorNumbers();
    }

    void MasterSendActorNumbers()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            return;
        }
        PV.RPC("RPC_OtherReceiveActorNumbers", RpcTarget.Others, actorNumbers);
    }

    [PunRPC]
    void RPC_OtherReceiveActorNumbers(int[] _actorNumbers)
    {
        actorNumbers = _actorNumbers;
        canSpawn = true;
    }
}