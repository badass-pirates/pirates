using UnityEngine;
using Photon.Pun;

public class GambleManager : MonoBehaviour
{
    public static GambleManager instance;
    void Awake()
    {
        if (instance != null) return;
        instance = this;

        NM = FindObjectOfType<GambleNetworkManager>();
        players.Add(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    const int MAX_DECIDE_TIME = 60, MAX_ATTACK_TIME = 60;
    const int MAX_ROUND = 3, MAX_ACT = 5;
    const int POT_WEIGHT = 1;

    public static GambleNetworkManager NM;


    public static PlayerInfoList players { get; set; } = new PlayerInfoList();
    public static GamblePlayer localPlayer { get; private set; } = null;
    public static State state { get; set; } = State.initial;

    public static int round { get; private set; } = 1;
    public static int act { get; private set; } = 1;
    public static int potCoins { get; set; } = 0;
    public static int chestCoins { get; set; }
    public static float leftTime { get; set; }
    float coinTime = 0f;

    void Update()
    {
        Debug.Log(state);
        switch (state)
        {
            case State.initial:
                OnInitial();
                break;
            case State.standBy:
                OnStandBy();
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
                coinTime = 0;
                break;
            case State.end:
            case State.loading:
                break;
        }
        coinTime += Time.deltaTime;
        if (coinTime > 5f) localPlayer.RemoveCoins();
    }

    private void OnInitial()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            state = State.loading;
            return;
        }
        if (players.Count() == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            state = State.loading;
            NM.SendPlayersToOthers(players);
            NM.SetState(State.standBy);
        }
    }

    private int GeneratePotCoins()
    {
        return Random.Range(GetMinPotCoins(), GetMaxPotCoins());
    }

    private int GetMinPotCoins()
    {
        int weight = (int)Mathf.Pow(POT_WEIGHT, round - 1);
        return weight * 10;
    }

    private int GetMaxPotCoins()
    {
        return GetMinPotCoins() * (round + 1);
    }

    private void OnStandBy()
    {
        localPlayer.SpawnMedals();
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;

        potCoins += GeneratePotCoins();
        NM.SendPotCoinsToOthers(potCoins);

        players.Reset();
        NM.SendPlayersToOthers(players);

        NM.SetTimer(MAX_DECIDE_TIME);
        NM.SetState(State.decide);
    }

    private void OnDecide()
    {
        if (leftTime > 0)
        {
            if (!players.EveryDecided())
            {
                leftTime -= Time.deltaTime;
                return;
            }
        }
        state = State.loading;

        players.ChangeUndecidedPlayerToShare();
        NM.SendPlayersToOthers(players);
        NM.SetState(State.check);
    }

    private void OnCheck()
    {
        localPlayer.DestroyMedals();
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;

        players.DecideChallengeWinner(potCoins);
        players.DecideAttackWinner();
        PlayerInfo attacker = players.GetAttackWinner();
        if (attacker != null)
        {
            attacker.SuccessChoiceAttack();
            NM.SendPlayersToOthers(players);
            NM.SetTimer(MAX_ATTACK_TIME);
            NM.SetState(State.attack);
            return;
        }
        NM.SendPlayersToOthers(players);
        NM.SetState(State.apply);
    }

    private void OnAttack()
    {
        if (leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            return;
        }
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;
        NM.SendPlayersToOthers(players);
        NM.SetState(State.apply);
    }

    // 다른 곳에서 GambleManager.Attack을 실행시켜야함
    // 이를 통해 다음 State로 넘어감
    public static void Attack(PlayerInfo target)
    {
        PlayerInfo attacker = players.GetAttackWinner();
        attacker.Attack(target);
        NM.SendPlayersToOthers(players);
        NM.SetState(State.apply);
    }

    private void OnApply()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            state = State.loading;
            return;
        }
        SetPlayerRewards();
        if (round >= MAX_ROUND && act >= MAX_ACT)
        {
            NM.SetState(State.end);
            return;
        }
        NM.EndAct();
    }

    private void SetPlayerRewards()
    {
        PlayerInfo challengeWinner = players.GetChallengeWinner();
        if (challengeWinner != null && challengeWinner.isLive)
        {
            challengeWinner.ChallengeWin();
            potCoins -= challengeWinner.challengeAmount;
        }
        players.ShareCoins(potCoins);
        potCoins %= players.Count();

        NM.SendPlayersToOthers(players);
        NM.SendPotCoinsToOthers(potCoins);
    }

    public static void Reward()
    {
        int winCoins = players.GetMine().coins - localPlayer.coinSpawner.transform.childCount - chestCoins;
        localPlayer.AddCoins(winCoins);
    }

    public static void NextAct()
    {
        if (act % MAX_ACT == 0)
            round++;
        act = (act % MAX_ACT) + 1;
    }

    public static void SetLocalPlayer(GamblePlayer player)
    {
        localPlayer = player;
    }

    public static void SetPlayerChoice(Choice choice)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            players.GetMine().SetChoice(choice);
            return;
        }
        NM.Choice(choice);
    }

    public static void SetPlayerChallengeAmount(int amount)
    {
        players.GetMine().SetChallengeAmount(amount);
    }

    public static PlayerInfo GetMyInfo()
    {
        return players.GetMine();
    }
}
