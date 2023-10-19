using Photon.Pun;
using Photon.Realtime;

// 네트워크와 관련하여 게임의 흐름을 관리하는 클래스
public class GambleNetworkManager : NetworkManager
{
    private void Start()
    {
        // 액터 넘버 초기화
        InitActorNumbers();

        // 로컬 플레이어가 마스터 클라이언트인 경우 액터 넘버 초기화, 스폰 허용, 다른 플레이어에게 알림
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                InsertActorNumber(player.ActorNumber);
            }
            canSpawn = true;
            MasterSendActorNumbers();
        }

        // 플레이어를 스폰하는 코루틴 시작
        StartCoroutine(SpawnPlayer());
    }

    // 액터 넘버 초기화
    protected override void InitActorNumbers()
    {
        int size = PhotonNetwork.CurrentRoom.PlayerCount;
        actorNumbers = new int[size];

        // 액터 넘버를 -1로 초기화
        for (int i = 0; i < actorNumbers.Length; i++)
        {
            actorNumbers[i] = -1;
        }
    }

    // 로컬 및 네트워크 플레이어의 리소스 이름 반환
    protected override (string, string) GetResourceName()
    {
        string localPlayer = "Local Player";
        string networkPlayer = "Network Player";
        return (localPlayer, networkPlayer);
    }

    // 모든 플레이어에게 게임 상태 설정
    public void SetStateToAll(State state)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveState", RpcTarget.AllViaServer, state);
    }

    // 게임 상태를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveState(State state)
    {
        // 모든 클라이언트에서 게임 상태 업데이트
        GambleManager.state = state;
    }

    // 마스터에게 로컬 플레이어의 액터 넘버 전송
    public void SendActorNumberToMaster()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveActorNumber", RpcTarget.MasterClient, actorNumber);
    }

    // 액터 넘버를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveActorNumber(int actorNumber)
    {
        // 플레이어 목록에 액터 넘버 추가
        GambleManager.players.Add(actorNumber);
    }

    // 다른 플레이어에게 플레이어 정보 전송
    public void SendPlayersToOthers(PlayerInfoList _players)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        (string players, string winner, string attacker) = _players.ToJson();
        PV.RPC("RPC_ReceivePlayers", RpcTarget.Others, players, winner, attacker);
    }

    // 플레이어 정보를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceivePlayers(string players, string winner, string attacker)
    {
        // 플레이어 정보 업데이트
        GambleManager.players = PlayerInfoList.FromJson(players, winner, attacker);
    }

    // 다른 플레이어에게 로그 메시지 전송
    public void SendLogToOthers(string message)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveLogMessage", RpcTarget.Others, message);
    }

    // 로그 메시지를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveLogMessage(string message)
    {
        // 로그 보드에 메시지 출력
        GambleManager.LogOnBoard(message);
    }

    // 다른 플레이어에게 포트 코인 전송
    public void SendPotCoinsToOthers(int coins)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceivePotCoins", RpcTarget.Others, coins);
    }

    // 포트 코인을 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceivePotCoins(int coins)
    {
        // 포트 코인 업데이트
        GambleManager.potCoins = coins;
    }

    // 마스터에게 로컬 플레이어의 선택 전송
    public void SendChoiceToMaster(Choice choice)
    {
        if (PhotonNetwork.IsMasterClient) return;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveChoice", RpcTarget.MasterClient, actorNumber, choice);
    }

    // 선택을 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveChoice(int actorNumber, Choice choice)
    {
        // 플레이어의 선택 업데이트
        GambleManager.players.SetPlayerChoice(actorNumber, choice);
    }

    // 마스터에게 로컬 플레이어의 도전 금액 전송
    public void SendChallengeAmountToMaster(int amount)
    {
        if (PhotonNetwork.IsMasterClient) return;
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        PV.RPC("RPC_ReceiveChallengeAmount", RpcTarget.MasterClient, actorNumber, amount);
    }

    // 도전 금액을 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveChallengeAmount(int actorNumber, int amount)
    {
        // 도전 금액 업데이트
        GambleManager.players.DecidePlayerChallenge(actorNumber, amount);
    }

    // 모든 플레이어에게 타이머 시간 설정
    public void SetTimerToAll(int time)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_ReceiveLeftTime", RpcTarget.AllViaServer, time);
    }

    // 타이머 시간을 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveLeftTime(int time)
    {
        // 남은 시간 업데이트
        GambleManager.leftTime = time;
    }

    // 마스터에게 로컬 플레이어의 공격 대상 전송
    public void SendAttackTargetToMaster(int targetActorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터에서 공격 처리
            GambleManager.AttackOnMaster(targetActorNumber);
            return;
        }
        PV.RPC("RPC_ReceiveAttackTarget", RpcTarget.MasterClient, targetActorNumber);
    }

    // 공격 대상을 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveAttackTarget(int targetActorNumber)
    {
        // 마스터에서 공격 처리
        GambleManager.AttackOnMaster(targetActorNumber);
    }

    // 액트 종료
    public void EndAct()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PV.RPC("RPC_EndAct", RpcTarget.AllViaServer);
    }

    // 액트 종료를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_EndAct()
    {
        // 보상 지급, 다음 액트 시작, 상태 설정
        GambleManager.Reward();
        GambleManager.NextAct();
        GambleManager.state = State.standBy;
    }

    // 액터 넘버를 받아오는 RPC 메서드
    [PunRPC]
    protected override void RPC_ReceiveActorNumbers(int[] _actorNumbers)
    {
        base.RPC_ReceiveActorNumbers(_actorNumbers);
    }

    // 공격 결과를 모든 플레이어에게 전송
    internal void SendAttackResultToAll(PlayerInfoList _players, int targetActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        (string players, string winner, string attacker) = _players.ToJson();
        PV.RPC("RPC_ReceiveAttackResult", RpcTarget.All, players, targetActorNumber);
    }

    // 공격 결과를 받아오는 RPC 메서드
    [PunRPC]
    private void RPC_ReceiveAttackResult(string players, int targetActorNumber)
    {
        // 플레이어 정보 업데이트
        GambleManager.players = PlayerInfoList.FromJson(players);

        // 만약 공격 대상이 자신인 경우 플레이어 제거
        if (GambleManager.GetMyInfo().actorNumber == targetActorNumber)
        {
            PhotonNetwork.Destroy(spawnedPlayer);
        }
    }
}
