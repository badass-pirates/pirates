using UnityEngine;
using System.Collections;
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


    public PotMoneySpawner potMoneySpawner;
    public static PlayerInfoList players { get; set; } = new PlayerInfoList();
    public static GamblePlayer localPlayer { get; private set; } = null;
    public static State state { get; set; } = State.initial;

    public static int round { get; private set; } = 1;
    public static int act { get; private set; } = 1;
    public static int potCoins { get; set; } = 0;
    public static int chestCoins { get; set; }
    public static float leftTime { get; set; }

    void Update()
    {
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
                StartCoroutine(RemoveCoins());
                break;
            case State.end:
            case State.loading:
                break;
        }
    }

    private IEnumerator RemoveCoins()
    {
        yield return new WaitForSeconds(5);
        localPlayer.RemoveCoins();
        yield break;
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
            NM.SetStateToAll(State.standBy);
        }
    }

    private int GeneratePotCoins()
    {
        return Random.Range(GetMinPotCoins(), GetMaxPotCoins());
    }

    public static int GetMinPotCoins()
    {
        int weight = (int)Mathf.Pow(POT_WEIGHT, round - 1);
        return weight * 10;
    }

    public static int GetMaxPotCoins()
    {
        return GetMinPotCoins() * (round + 1);
    }

    private void OnStandBy()
    {
        localPlayer.SpawnMedals();
        potMoneySpawner.DestroyPot();
        potMoneySpawner.SpawnPot(localPlayer.transform, round);
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;

        potCoins += GeneratePotCoins();
        NM.SendPotCoinsToOthers(potCoins);

        players.Reset();
        NM.SendPlayersToOthers(players);

        NM.SetTimerToAll(MAX_DECIDE_TIME);
        NM.SetStateToAll(State.decide);
    }

    private void OnDecide()
    {
        if (leftTime > 0 && !players.EveryDecided())
        {
            leftTime -= Time.deltaTime;
            return;
        }
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;

        players.ChangeUndecidedPlayerToShare();
        NM.SendPlayersToOthers(players);
        NM.SetStateToAll(State.check);
    }

    public static void DecideChoice(Choice choice)
    {
        localPlayer.DestroyMedalsWithEffect(choice);
        if (PhotonNetwork.IsMasterClient)
        {
            players.GetMine().DecideChoice(choice);
            return;
        }
        NM.SendChoiceToMaster(choice);
    }

    public static void DecidePlayerChallenge(int amount)
    {
        localPlayer.DestroyMedalsWithEffect(Choice.challenge);
        if (PhotonNetwork.IsMasterClient)
        {
            players.GetMine().DecideChallenge(amount);
            return;
        }
        NM.SendChallengeAmountToMaster(amount);
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
            NM.SetTimerToAll(MAX_ATTACK_TIME);
            NM.SetStateToAll(State.attack);
            return;
        }
        NM.SendPlayersToOthers(players);
        NM.SetStateToAll(State.apply);
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
        NM.SetStateToAll(State.apply);
    }

    // 다른 곳에서 GambleManager.Attack을 실행시켜야함
    // 이를 통해 다음 State로 넘어감
    public static void Attack(PlayerInfo target)
    {
        PlayerInfo attacker = players.GetAttackWinner();
        attacker.Attack(target);
        NM.SendPlayersToOthers(players);
        NM.SetStateToAll(State.apply);
    }

    private void OnApply()
    {
        state = State.loading;
        if (!PhotonNetwork.IsMasterClient) return;

        SetPlayerRewards();
        if (round >= MAX_ROUND && act >= MAX_ACT)
        {
            NM.SetStateToAll(State.end);
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
        if (PhotonNetwork.IsMasterClient) return;
        NM.SendActorNumberToMaster();
    }

    public static PlayerInfo GetMyInfo()
    {
        return players.GetMine();
    }
}
