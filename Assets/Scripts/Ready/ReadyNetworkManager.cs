using Photon.Pun;
using Photon.Realtime;

public class ReadyNetworkManager : NetworkManager
{
    // 플레이어가 방에 입장했을 때 호출되는 함수
    public override void OnJoinedRoom()
    {
        // 기본 OnJoinedRoom 함수 호출
        base.OnJoinedRoom();

        // actorNumbers 배열이 초기화되지 않았다면 초기화
        if (actorNumbers == null)
        {
            InitActorNumbers();
        }

        // 마스터 클라이언트인 경우
        if (PhotonNetwork.IsMasterClient)
        {
            // 로컬 플레이어의 액터 번호를 actorNumbers 배열에 추가
            InsertActorNumber(PhotonNetwork.LocalPlayer.ActorNumber);
            // 플레이어 스폰이 가능한 상태로 설정
            canSpawn = true;
        }

        // 일정 시간 후에 플레이어 스폰을 위한 코루틴 실행
        StartCoroutine(SpawnPlayer());
    }

    // 다른 플레이어가 방에 입장했을 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 입장한 플레이어의 액터 번호 가져오기
        int actorNumber = otherPlayer.ActorNumber;

        // actorNumbers 배열에 액터 번호 추가
        InsertActorNumber(actorNumber);

        // 마스터 클라이언트에게 새로운 액터 번호를 전달
        MasterSendActorNumbers();
    }

    // actorNumbers 배열 초기화 함수
    protected override void InitActorNumbers()
    {
        // 길이가 8이고 초기값이 -1인 actorNumbers 배열 생성
        actorNumbers = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
    }

    // 리소스 이름 가져오는 함수
    protected override (string, string) GetResourceName()
    {
        // 로컬 플레이어 및 네트워크 플레이어의 리소스 이름 반환
        string localPlayer = "Ready/Local Player";
        string networkPlayer = "Network Player";
        return (localPlayer, networkPlayer);
    }

    // 액터 번호 수신 함수 (PunRPC 어트리뷰트 사용)
    [PunRPC]
    protected override void RPC_ReceiveActorNumbers(int[] _actorNumbers)
    {
        // 기본 RPC_ReceiveActorNumbers 함수 호출
        base.RPC_ReceiveActorNumbers(_actorNumbers);
    }
}