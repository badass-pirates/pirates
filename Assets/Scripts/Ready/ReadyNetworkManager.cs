using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ReadyNetworkManager : NetworkManager
{
    protected override void InitActorNumbers()
    {
        actorNumbers = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    }

    protected override IEnumerator SpawnPlayer()
    {
        yield return new WaitUntil(() => canSpawn);

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = Array.FindIndex<int>(actorNumbers, x => x == actorNumber);
        Transform startPoint = CalculateStartPoint(index);
        if (PV.IsMine)
        {
            GameObject localPlayer = Instantiate(Resources.Load<GameObject>("Ready/Local Player"), startPoint.position, startPoint.rotation);
        }
        spawnedPlayer = PhotonNetwork.Instantiate("Ready/Network Player", startPoint.position, startPoint.rotation);
        yield break;
    }

    protected override Transform CalculateStartPoint(int index)
    {
        GameObject empty = new GameObject();
        Transform startPoint = empty.transform;
        startPoint.Rotate(new Vector3(0, 360 / 8 * index, 0)); // 필요한 각도만큼 회전
        startPoint.Translate(new Vector3(0, 0, -3f)); // 테이블 넓이만큼 후방으로 이동
        return startPoint;
    }
}
