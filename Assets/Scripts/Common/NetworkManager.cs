using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// 네트워크 환경을 제어하는 스크립트
public abstract class NetworkManager : MonoBehaviourPunCallbacks
{
    // PhotonView 컴포넌트에 대한 참조
    public PhotonView PV;

    // 플레이어 간의 시작 위치 간격
    public float distance;

    // 생성된 플레이어에 대한 참조
    protected GameObject spawnedPlayer;

    // 플레이어 액터 번호 배열
    protected int[] actorNumbers;

    // 플레이어 스폰 가능 여부
    protected bool canSpawn = false;

    // 액터 번호 배열 초기화 추상 메서드
    protected abstract void InitActorNumbers();

    // 플레이어 스폰 코루틴
    protected IEnumerator SpawnPlayer()
    {
        // canSpawn이 true일 때까지 대기
        yield return new WaitUntil(() => canSpawn);

        // 로컬 플레이어 및 네트워크 플레이어 리소스 이름 가져오기
        (string localPlayer, string networkPlayer) = GetResourceName();
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = Array.FindIndex<int>(actorNumbers, x => x == actorNumber);

        // 시작 위치 계산 및 플레이어 생성
        Transform startPoint = CalculateStartPoint(index);
        Instantiate(Resources.Load<GameObject>(localPlayer), startPoint.position, startPoint.rotation);
        spawnedPlayer = PhotonNetwork.Instantiate(networkPlayer, startPoint.position, startPoint.rotation);
        yield break;
    }

    // 로컬 및 네트워크 플레이어의 리소스 이름을 반환하는 추상 메서드
    protected abstract (string, string) GetResourceName();

    // 시작 위치 계산 메서드
    protected Transform CalculateStartPoint(int index)
    {
        GameObject empty = new GameObject();
        Transform startPoint = empty.transform;

        // 원형으로 퍼져있는 시작 각도 설정
        startPoint.Rotate(new Vector3(0, 360 / actorNumbers.Length * index, 0));

        // 거리만큼 뒤로 이동
        startPoint.Translate(new Vector3(0, 0, distance * -1));

        return startPoint;
    }

    // 플레이어 액터 번호를 배열에 추가하는 메서드
    protected void InsertActorNumber(int actorNumber)
    {
        int index = Array.FindIndex<int>(actorNumbers, x => x == -1);
        actorNumbers[index] = actorNumber;
    }

    // 플레이어가 방을 나갈 때 호출되는 함수
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (spawnedPlayer != null)
            PhotonNetwork.Destroy(spawnedPlayer);
    }

    // 다른 플레이어가 방을 나갈 때 호출되는 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 로컬 플레이어가 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            return;
        }
        // 마스터 클라이언트가 다른 플레이어의 정보를 제거하는 메서드 호출
        MasterRemovePlayerInfo(otherPlayer.ActorNumber);
    }

    // 마스터 클라이언트가 다른 플레이어의 정보를 제거하는 메서드
    protected void MasterRemovePlayerInfo(int actorNum)
    {
        int index = Array.FindIndex<int>(actorNumbers, x => x == actorNum);
        actorNumbers[index] = -1;
        MasterSendActorNumbers();
    }

    // 마스터 클라이언트가 플레이어들의 액터 번호를 전달하는 메서드
    protected void MasterSendActorNumbers()
    {
        // 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        // RPC를 사용하여 다른 플레이어에게 액터 번호 전달
        PV.RPC("RPC_ReceiveActorNumbers", RpcTarget.Others, actorNumbers);
    }

    // 액터 번호를 수신하는 RPC 메서드
    [PunRPC]
    protected virtual void RPC_ReceiveActorNumbers(int[] _actorNumbers)
    {
        actorNumbers = _actorNumbers; // 액터 번호 업데이트
        canSpawn = true; // 스폰 가능 상태로 변경
    }
}
