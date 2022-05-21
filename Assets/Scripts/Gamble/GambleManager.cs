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
        challengeWinner = null;
        attackWinner = null;
        victim = null;
        round = 1;
        act = 1;
        potCoins = 0;
        coinTime = 0f;

        state = State.initial;

        localPlayer = null;

        //싱글플레이 위한 초기화
        playerList.Add(new PlayerInfo(PhotonNetwork.LocalPlayer.ActorNumber));
    }

    const int MAX_DECIDE_TIME = 60, MAX_ATTACK_TIME = 60;
    const int MAX_ROUND = 3, MAX_ACT = 5;
    const int POT_WEIGHT = 1;


    public static State state { get; private set; }

    public static GamblePlayer localPlayer {get; private set;}
    public static PlayerInfo localPlayerInfo { get; private set; }

    static List<PlayerInfo> playerList = new List<PlayerInfo>();
    static PlayerInfo challengeWinner, attackWinner, victim;

    public static int round { get; private set; }
    public static int act { get; private set; }
    public static int potCoins { get; private set; }
    public static int chestCoins {get; set; }
    public static float leftTime { get; private set; }
    float coinTime;

    void StandBy()
    {
        if (localPlayer == null) return;
        Debug.Log(localPlayer);
        ResetPlayerInfoList();
        potCoins += GetPotCoins();
        leftTime = MAX_DECIDE_TIME;
        state = State.decide;
        localPlayer.SpawnMedals();
    }
    void Decide()
    {
        if (leftTime > 0)
        {
            foreach (var p in playerList)
            {
                if (p.choice == Choice.none)
                {
                    leftTime -= Time.deltaTime;
                    return;
                }
            }
        }
        else
        {
            foreach (var p in playerList)
                if (p.choice == Choice.none) p.choice = Choice.share;
        }
        state = State.check;
    }
    void Check()
    {
        localPlayer.DestroyMedals();
        SetChallengeWinner();
        SetAttackWinner();
        state = State.apply;
    }
    void Attack()
    {
        if (!attackWinner.canShoot || leftTime > 0)
        {
            leftTime -= Time.deltaTime;
        }
        else state = State.apply;
        //if kill?
        //사격 시 대상이 플레이어인 경우
        { victim = playerList[4]; }
        attackWinner.canShoot = false;
        attackWinner.attackChance--;

    }
    void Apply()
    {
        SetPlayerRewards();
        //네트워크에 각자 T 결과에 따라 처리하도록 전송
        if (MAX_ROUND <= round && MAX_ACT <= act)
        {
            state = State.end;
            return;
        }
        int winCoins = localPlayerInfo.coins - localPlayer.coinSpawner.transform.childCount - chestCoins;
        localPlayer.AddCoins(winCoins);
        state = State.standBy;
        if (act % MAX_ACT == 0)
            round++;
        act = (act % MAX_ACT) + 1;
    }

    //Player Input
    public static void SetLocalPlayer(GamblePlayer player)
    {
        int aNum = PhotonNetwork.LocalPlayer.ActorNumber;
        localPlayer = player;
        //localPlayerInfo = GetPlayerInfo(aNum);
        localPlayerInfo = playerList[0];
        state = State.standBy;
    }
    public static void SetPlayerChoice(Choice choice)
    {
        if (localPlayerInfo != null)
            localPlayerInfo.choice = choice;
        else Debug.Log("localPlayer is null!");
    }
    public static void SetPlayerChallengeAmount(int amount)
    {
        if (localPlayerInfo != null)
            localPlayerInfo.challengeAmount = amount;
        else Debug.Log("localPlayer is null!");
    }

    //Network PlayerInfo
    public static void SetPlayerList(string orArray)
    {
        //미구현
        int aNum = 0;//임의의 액터넘버
        playerList.Add(new PlayerInfo(aNum));
        state = State.standBy;
    }
    public static void SetPlayerInfo(int aNum, string data)
    {
        PlayerInfo player = playerList.Find(p => p.actorNumber == aNum);
        //json 파싱
    }
    public static PlayerInfo GetPlayerInfo(int aNum)
    {
        return playerList.Find(p => p.actorNumber == aNum);
    }
    //StandBy State
    static void ResetPlayerInfoList()
    {
        foreach (var p in playerList)
        {
            p.choice = Choice.none;
            p.challengeAmount = 0;
            p.canShoot = false;
        }
        victim = null;
        challengeWinner = null;
        attackWinner = null;
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
    int GetPotCoins()
    {
        return Random.Range(GetMinPotCoins(), GetMaxPotCoins());
    }
    bool IsInPotRange(int amount)
    {
        if (GetMinPotCoins() <= amount)
        {
            if (GetMaxPotCoins() >= amount)
                return true;
        }
        return false;
    }

    //Check State
    void SetChallengeWinner()
    {
        challengeWinner = null;
        List<PlayerInfo> list = (from p in playerList
                                 where p.choice == Choice.challenge && IsInPotRange(p.challengeAmount)
                                 select p).ToList<PlayerInfo>();

        if (list.Count > 0)
        {
            int maxAmount = list.Max(p => p.challengeAmount);
            challengeWinner = list.Find(p => p.challengeAmount == maxAmount);
        }
    }

    void SetAttackWinner()
    {
        attackWinner = null;

        var list = (from p in playerList
                    where p.choice == Choice.attack
                    select p).ToList<PlayerInfo>();

        if (list.Count == 1)
        {
            attackWinner = list.Find(p => p.choice == Choice.attack);
            state = State.attack;
        }

    }

    //Apply State
    void KillPlayer(PlayerInfo victim)
    {
        if (victim == null) return;
        attackWinner.coins += victim.coins;
        victim.coins = 0;
        victim.isLive = false;
    }

    int SharePlayer()
    {
        var list = (from p in playerList
                    where p.choice == Choice.share
                    && p.isLive
                    select p).ToList<PlayerInfo>();

        foreach (var p in list)
        {
            p.coins += potCoins / list.Count;
        }

        if (list.Count > 0)
            return potCoins % list.Count;

        return 0;
    }

    void SetPlayerRewards()
    {
        KillPlayer(victim);
        if (challengeWinner != null)
        {
            challengeWinner.coins += challengeWinner.challengeAmount;
            potCoins -= challengeWinner.challengeAmount;
        }
        potCoins = SharePlayer();
    }

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
        coinTime+= Time.deltaTime;
        if(coinTime > 5f) localPlayer.RemoveCoins();
    }
}
