using System.Collections;
using System.Linq;
using System.Collections.Generic;
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
    public static float leftTime { get; private set; }
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
                StandBy();
                break;
            case State.decide:
                Decide();
                break;
            case State.check:
                Check();
                break;
            case State.attack:
                Attack();
                break;
            case State.apply:
                Apply();
                coinTime = 0; 
                break;
            case State.end: 
                break;
        }
        coinTime += Time.deltaTime;
        if (coinTime > 5f) localPlayer.RemoveCoins();
    }

    private void OnInitial()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            NM.SendActorNumberToMaster();
            state = State.loading;
            return;
        }
        if (players.Count() == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            potCoins += GeneratePotCoins();
            NM.SendPotCoins(potCoins);
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

    private void StandBy()
    {
        leftTime = MAX_DECIDE_TIME;
        players.Reset();
        localPlayer.SpawnMedals();
        state = State.loading;
        if (PhotonNetwork.IsMasterClient)
        {
            NM.SetState(State.decide);
        }
    }

    private void Decide()
    {
        if (leftTime > 0)
        {
            if (!players.EveryDecided())
            {
                leftTime -= Time.deltaTime;
                return;
            }
        }
        players.ChangeUndecidedPlayerToShare();
        NM.SendPlayersToOthers(players);
        NM.SetState(State.check);
    }

    private void Check()
    {
        localPlayer.DestroyMedals();
        if (!PhotonNetwork.IsMasterClient)
        {
            state = State.loading;
            return;
        }
        players.DecideChallengeWinners(potCoins);
        players.DecideAttackWinner();
        PlayerInfo attacker = players.GetAttackWinner();
        if (attacker != null)
        {
            attacker.SuccessChoiceAttack();
            NM.SendPlayersToOthers(players);
            NM.SetState(State.attack);
            return;
        }
        NM.SendPlayersToOthers(players);
        NM.SetState(State.apply);
    }

    private void Attack()
    {
        PlayerInfo attacker = players.GetAttackWinner();
        if (attacker.canShoot && leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            return;
        }
        // 다른 곳에서 attacker.Attack(target)을 실행시켜야함
        // PlayerInfo target = new PlayerInfo();
        // attacker.Attack(target);
        state = State.apply;
    }

    private void Apply()
    {
        SetPlayerRewards();
        //네트워크에 각자 T 결과에 따라 처리하도록 전송
        if (MAX_ROUND <= round && MAX_ACT <= act)
        {
            state = State.end;
            return;
        }
        int winCoins = players.GetMine().coins - localPlayer.coinSpawner.transform.childCount - chestCoins;
        localPlayer.AddCoins(winCoins);
        state = State.standBy;
        if (act % MAX_ACT == 0)
            round++;
        act = (act % MAX_ACT) + 1;
    }

    private void SetPlayerRewards()
    {
        PlayerInfo challengeWinner = players.GetChallengeWinner();
        if (challengeWinner != null && challengeWinner.isLive)
        {
            challengeWinner.Win();
            potCoins -= challengeWinner.challengeAmount;
        }
        players.ShareCoins(potCoins);
        potCoins %= players.Count();
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
