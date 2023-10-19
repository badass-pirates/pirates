using UnityEngine;
using System.Collections;
using Photon.Pun;

// 게임의 전반적인 흐름을 관리하는 클래스
public class GambleManager : MonoBehaviour
{
    public static GambleManager instance;

    // 게임 매니저 인스턴스
    void Awake()
    {
        if (instance != null) return;
        instance = this;
        NM = FindObjectOfType<GambleNetworkManager>();
        players.Add(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    // 상수 값 정의
    const int MAX_DECIDE_TIME = 60, MAX_ATTACK_TIME = 60;
    const int MAX_ROUND = 1, MAX_ACT = 4;
    const int POT_WEIGHT = 1;

    // 게임 네트워크 매니저
    public static GambleNetworkManager NM;

    // UI 매니저
    public UIManager UIM;

    // 돈 뿌리기 스포너
    public PotMoneySpawner potMoneySpawner;

    // 선택 영역을 나타내는 게임 오브젝트
    public GameObject choiceZone;

    // 플레이어 정보 리스트
    public static PlayerInfoList players { get; set; } = new PlayerInfoList();

    // 로컬 플레이어 객체
    public static GamblePlayer localPlayer { get; private set; } = null;

    // 현재 게임 상태
    public static State state { get; set; } = State.initial;

    // 현재 라운드
    public static int round { get; private set; } = 1;

    // 현재 액트
    public static int act { get; private set; } = 1;

    // 현재 포트 코인
    public static int potCoins { get; set; } = 0;

    // 보관함 코인
    public static int chestCoins { get; set; }

    // 남은 시간
    public static float leftTime { get; set; }

    // 업데이트 함수
    void Update()
    {
        switch (state)
        {
            case State.initial:
                OnInitial();
                break;
            case State.standBy:
                StartCoroutine(OnStandBy());
                break;
            case State.decide:
                OnDecide();
                break;
            case State.check:
                OnCheck();
                break;
            case State.attack:
                OnAttack();
                break;
            case State.apply:
                OnApply();
                StartCoroutine(RemoveCoins());
                break;
            case State.end:
                OnEnd();
                break;
            case State.loading:
                break;
        }
    }

    // 코인 제거 코루틴
    private IEnumerator RemoveCoins()
    {
        yield return new WaitForSeconds(5);
        localPlayer.RemoveCoins();
        yield break;
    }

    // 초기 상태 처리 함수
    private void OnInitial()
    {
        // 마스터 클라이언트가 아닌 경우 상태를 로딩으로 변경하고 종료
        if (!PhotonNetwork.IsMasterClient)
        {
            state = State.loading;
            return;
        }

        // 모든 플레이어가 입장한 경우
        if (players.Count() == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            state = State.loading;

            // 다른 플레이어들에게 자신의 정보를 전송
            NM.SendPlayersToOthers(players);

            // 모든 플레이어들에게 준비 상태로 변경 요청 전송
            NM.SetStateToAll(State.standBy);
        }
    }

    // 포트 코인 생성 함수
    private int GeneratePotCoins()
    {
        return Random.Range(GetMinPotCoins(), GetMaxPotCoins());
    }

    // 최소 포트 코인 얻기 함수
    public static int GetMinPotCoins()
    {
        int weight = (int)Mathf.Pow(POT_WEIGHT, round - 1);
        return weight * 10;
    }

    // 최대 포트 코인 얻기 함수
    public static int GetMaxPotCoins()
    {
        return GetMinPotCoins() * (round + 1);
    }

    // 준비 상태 처리 코루틴
    private IEnumerator OnStandBy()
    {
        // 선택 영역 활성화
        choiceZone.SetActive(true);
        state = State.loading;

        // 포트 제거
        potMoneySpawner.DestroyPot();
        yield return new WaitForSeconds(1);

        // 로컬 플레이어가 살아있을 경우 메달과 로그 보드 생성
        if (GetMyInfo().isLive)
        {
            localPlayer.SpawnMedals();
            localPlayer.SpawnLogBoard();
        }

        // 포트 생성
        potMoneySpawner.SpawnPot(localPlayer.transform, round);

        // 액트 시작 메시지 로그
        if (act == 1)
        {
            localPlayer.LogOnBoard($"Round {round} start!");
        }
        localPlayer.LogOnBoard($"Act {act}");

        // 마스터 클라이언트인 경우 포트 코인 생성 및 전송
        if (PhotonNetwork.IsMasterClient)
        {
            potCoins += GeneratePotCoins();
            NM.SendPotCoinsToOthers(potCoins);

            // 플레이어 정보 초기화 및 전송
            players.Reset();
            NM.SendPlayersToOthers(players);

            // 다른 플레이어들에게 타이머 설정 및 선택 상태로 변경 요청 전송
            NM.SetTimerToAll(MAX_DECIDE_TIME);
            NM.SetStateToAll(State.decide);
        }
    }

    // 선택 상태 처리 함수
    private void OnDecide()
    {
        // 남은 시간이 있고 모든 플레이어가 선택을 완료하지 않았을 경우
        if (leftTime > 0 && !players.EveryDecided())
        {
            leftTime -= Time.deltaTime;
            return;
        }

        // 로딩 상태로 변경
        state = State.loading;

        // 마스터 클라이언트인 경우 선택하지 않은 플레이어를 Share로 변경하고 정보 전송
        if (PhotonNetwork.IsMasterClient)
        {
            players.ChangeUndecidedPlayerToShare();
            NM.SendPlayersToOthers(players);
            NM.SetStateToAll(State.check);
        }
    }

    // 선택 완료 함수
    public static void DecideChoice(Choice choice)
    {
        localPlayer.DestroyMedalsWithEffect(choice);

        // 마스터 클라이언트인 경우 로컬 플레이어의 선택을 갱신하고 정보 전송
        if (PhotonNetwork.IsMasterClient)
        {
            players.GetMine().DecideChoice(choice);
            return;
        }

        // 마스터 클라이언트가 아닌 경우 선택된 항목을 마스터 클라이언트에게 전송
        NM.SendChoiceToMaster(choice);
    }

    // 도전 금액 선택 함수
    public static void DecidePlayerChallenge(int amount)
    {
        localPlayer.DestroyMedalsWithEffect(Choice.challenge);

        // 마스터 클라이언트인 경우 로컬 플레이어의 도전 금액 선택을 갱신하고 정보 전송
        if (PhotonNetwork.IsMasterClient)
        {
            players.GetMine().DecideChallenge(amount);
            return;
        }

        // 마스터 클라이언트가 아닌 경우 도전 금액을 마스터 클라이언트에게 전송
        NM.SendChallengeAmountToMaster(amount);
    }

    // 확인 상태 처리 함수
    private void OnCheck()
    {
        localPlayer.DestroyMedals();
        state = State.loading;

        // 마스터 클라이언트인 경우 도전 결과 및 공격 결과를 확인하고 정보 전송
        if (PhotonNetwork.IsMasterClient)
        {
            players.DecideChallengeWinner(potCoins);
            PlayerInfo challengeWinner = players.GetChallengeWinner();

            // 도전 성공한 플레이어에게 로그 메시지 전송
            if (challengeWinner != null)
            {
                NM.SendLogToOthers($"{challengeWinner.name} success to choice challenge!");
            }
            else
            {
                // 도전 실패한 플레이어들에게 로그 메시지 전송
                players.GetChallengersName()
                    .ForEach(name => NM.SendLogToOthers($"{name} fail to choice challenge!"));
            }

            // 공격 결과 확인 및 처리
            players.DecideAttackWinner();
            PlayerInfo attacker = players.GetAttackWinner();

            // 공격 성공한 플레이어에게 로그 메시지 전송
            if (attacker != null)
            {
                attacker.SuccessChoiceAttack();
                NM.SendLogToOthers($"{attacker.name} success to choice attack!");
                NM.SendPlayersToOthers(players);
                NM.SetTimerToAll(MAX_ATTACK_TIME);
                NM.SetStateToAll(State.attack);
                return;
            }

            // 공격 실패한 플레이어들에게 로그 메시지 전송
            players.GetAttackersName()
                .ForEach(name => NM.SendLogToOthers($"{name} fail to choice attack!"));

            // 다른 플레이어들에게 플레이어 정보 전송 및 상태 변경
            NM.SendPlayersToOthers(players);
            NM.SetStateToAll(State.apply);
        }
    }

    // 공격 상태 처리 함수
    private void OnAttack()
    {
        choiceZone.SetActive(false);
        potMoneySpawner.DestroyPot();

        // 로컬 플레이어가 쏠 수 있는 경우 총 생성
        if (players.GetMine().canShoot)
        {
            localPlayer.SpawnGun();
            GetMyInfo().canShoot = false;
        }

        // 남은 시간이 있는 경우
        if (leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            return;
        }

        // 로컬 플레이어의 총 제거 및 로딩 상태로 변경
        localPlayer.DestroyGun();
        state = State.loading;

        // 마스터 클라이언트가 아닌 경우 다른 플레이어들에게 플레이어 정보 전송 및 상태 변경
        if (!PhotonNetwork.IsMasterClient) return;
        NM.SendPlayersToOthers(players);
        NM.SetStateToAll(State.apply);
    }

    // 공격 함수
    public static void Attack(int targetActorNumber)
    {
        NM.SendAttackTargetToMaster(targetActorNumber);
    }

    // 마스터에서 공격 처리 함수
    public static void AttackOnMaster(int targetActorNumber)
    {
        // 공격 대상이 없는 경우 상태를 적용으로 변경하고 종료
        if (targetActorNumber == -1)
        {
            NM.SetStateToAll(State.apply);
            return;
        }

        // 공격자와 대상 플레이어 정보 획득
        PlayerInfo attacker = players.GetAttackWinner();
        PlayerInfo target = players.Find(targetActorNumber);

        // 공격자 정보 갱신 및 결과 정보 전송
        attacker.Attack(target);
        NM.SendAttackResultToAll(players, targetActorNumber);
        NM.SetStateToAll(State.apply);
    }

    // 적용 상태 처리 함수
    private void OnApply()
    {
        state = State.loading;

        // 마스터 클라이언트가 아닌 경우 플레이어 보상 설정 및 정보 전송
        if (!PhotonNetwork.IsMasterClient) return;

        SetPlayerRewards();

        // 최대 라운드 및 액트에 도달한 경우 게임 종료 상태로 변경
        if (round >= MAX_ROUND && act >= MAX_ACT)
        {
            NM.SetStateToAll(State.end);
            return;
        }

        // 액트 종료 전송
        NM.EndAct();
    }

    // 플레이어 보상 설정 함수
    private void SetPlayerRewards()
    {
        // 도전에 성공한 플레이어 정보 획득
        PlayerInfo challengeWinner = players.GetChallengeWinner();

        // 도전에 성공하고 살아있는 경우 보상 설정
        if (challengeWinner != null && challengeWinner.isLive)
        {
            challengeWinner.ChallengeWin();
            potCoins -= challengeWinner.challengeAmount;
        }

        // 포트 코인을 플레이어들에게 공평하게 분배
        players.ShareCoins(potCoins);
        potCoins %= players.Count();

        // 플레이어 정보 및 포트 코인 정보 전송
        NM.SendPlayersToOthers(players);
        NM.SendPotCoinsToOthers(potCoins);
    }

    // 보상 함수
    public static void Reward()
    {
        // 플레이어의 보상 획득 및 로그 메시지 전송
        int winnings = GetMyInfo().winnings;
        if (winnings <= 0)
        {
            localPlayer.LogOnBoard($"No Coins!");
            return;
        }
        localPlayer.AddCoins(winnings);
        localPlayer.LogOnBoard($"You get {winnings} coins!");
    }

    // 다음 액트 설정 함수
    public static void NextAct()
    {
        // 액트가 최대 액트에 도달한 경우 라운드 증가
        if (act % MAX_ACT == 0)
            round++;

        // 액트 증가
        act = (act % MAX_ACT) + 1;
    }

    // 종료 상태 처리 함수
    private void OnEnd()
    {
        state = State.loading;
        localPlayer.LogOnBoard("The End!");
        UIM.SetEndingTextUI();
    }

    // 로컬 플레이어 설정 함수
    public static void SetLocalPlayer(GamblePlayer player)
    {
        localPlayer = player;
        if (PhotonNetwork.IsMasterClient) return;
        NM.SendActorNumberToMaster();
    }

    // 내 플레이어 정보 얻기 함수
    public static PlayerInfo GetMyInfo()
    {
        return players.GetMine();
    }

    // 종료 함수
    public void Quit()
    {
        Application.Quit();
    }

    // 로그 보드 함수
    public static void LogOnBoard(string message)
    {
        localPlayer.LogOnBoard(message);
    }
}
