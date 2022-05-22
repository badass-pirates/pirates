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
        instance = this;

        round = 1;
        act = 1;
        potCoins = 0;
        coinTime = 0f;

        state = State.initial;

        localPlayer = null;

        //싱글플레이 위한 초기화
        players.Add(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    const int MAX_DECIDE_TIME = 60, MAX_ATTACK_TIME = 60;
    const int MAX_ROUND = 3, MAX_ACT = 5;
    const int POT_WEIGHT = 1;


    public static State state { get; private set; }

    public static GamblePlayer localPlayer {get; private set;}

    private static PlayerInfoList players = new PlayerInfoList();

    public static int round { get; private set; }
    public static int act { get; private set; }
    public static int potCoins { get; private set; }
    public static int chestCoins { get; set; }
    public static float leftTime { get; private set; }
    float coinTime;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);
        switch (state)
        {
            case State.standBy: StandBy(); break;
            case State.decide: Decide(); break;
            case State.check: Check(); break;
            case State.attack: Attack(); break;
            case State.apply: Apply(); coinTime = 0; break;
            case State.end: break;
        }
        coinTime += Time.deltaTime;
        if (coinTime > 5f) localPlayer.RemoveCoins();
    }

    private void StandBy()
    {
        if (localPlayer == null) return;
        Debug.Log(localPlayer);

        players.Reset();
        potCoins += GetPotCoins();
        leftTime = MAX_DECIDE_TIME;
        state = State.decide;
        localPlayer.SpawnMedals();
    }

    private int GetPotCoins()
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
        else
        {
            players.ChangeUndecidedPlayerToShare();
        }
        state = State.check;
    }

    private void Check()
    {
        localPlayer.DestroyMedals();
        players.DecideChallengeWinners(potCoins);
        players.DecideAttackWinner();
        PlayerInfo attacker = players.GetAttackWinner();
        if (attacker != null)
        {
            attacker.SuccessChoiceAttack();
            state = State.attack;
            return;
        }
        state = State.apply;
    }

    private void Attack()
    {
        PlayerInfo attacker = players.GetAttackWinner();
        if (!attacker.canShoot || leftTime > 0)
        {
            leftTime -= Time.deltaTime;
        }
        else state = State.apply;
        //if kill?
        //사격 시 대상이 플레이어인 경우
        PlayerInfo target = new PlayerInfo();
        attacker.Attack(target);
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
        List<PlayerInfo> challengeWinners = players.GetChallengeWinners();
        if (challengeWinners.Count > 0)
        {
            int winnerCoins = challengeWinners.First().coins / challengeWinners.Count;
            challengeWinners.ForEach(winner => winner.AddCoin(winnerCoins));
            potCoins -= winnerCoins;
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
        players.GetMine().SetChoice(choice);
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
