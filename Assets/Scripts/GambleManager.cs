using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GambleManager : MonoBehaviour
{
    public static GambleManager instance;
    void Awake()
    {
        instance = this;
        challengeWinner = null;
        attackWinner = null;
        round = 1;
        act = 1;

        state = State.ready;

        potCoins = 0;
        maxPotCoins = 0;

        //싱글플레이 위한 초기화
        playerList.Add(new PlayerInfo());
        playerIndex = 0;
        victim = null;
    }

    static State state;

    const int POT_WEIGHT = 1;
    const int MAX_DECIDE_TIME = 60;
    const int MAX_ATTACK_TIME = 60;

    const int MAX_ROUND = 3;
    const int MAX_ACT = 5;

    float leftTime;
    public TextMeshProUGUI timeText;
    public GameObject playerInfoText, gambleInfoText;

    static GameObject localPlayer;

    static int round, act;
    static int potCoins;

    static public int maxPotCoins;

    static List<PlayerInfo> playerList = new List<PlayerInfo>();
    static public int playerIndex { get; set; }

    PlayerInfo challengeWinner, attackWinner, victim;

    void Ready()
    {
        if (localPlayer == null) return;

        ResetPlayerInfoData();
        potCoins += GetPotCoins(round);
        maxPotCoins = (int)Mathf.Pow(POT_WEIGHT, round - 1) * (round + 1);
        maxPotCoins += GetPotCoins(round);
        leftTime = MAX_DECIDE_TIME;
        state = State.decide;
        localPlayer.GetComponentInChildren<MedalSpawner>().SpawnMedals();
    }

    void Decide()
    {
        if (leftTime > 0)
        {
            foreach (var p in playerList)
            {
                if (p.choice == Choice.ready)
                {
                    leftTime -= Time.deltaTime;
                    timeText.text = ((int)leftTime).ToString();
                    return;
                }
            }
        }
        else
        {
            foreach (var p in playerList)
            {
                if (p.choice == Choice.ready) p.choice = Choice.share;
            }
        }
        state = State.check;
    }

    void Check()
    {
        localPlayer.GetComponentInChildren<MedalSpawner>().DestroyMedals();
        SetChallengeWinner();
        SetAttackWinner();
        state = State.apply;
    }

    void Attack()
    {
        if (!attackWinner.canShoot || leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            timeText.text = ((int)leftTime).ToString();
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
        state = State.ready;
        if (act % MAX_ACT == 0)
            round++;
        act = (act % MAX_ACT) + 1;
    }

    void Update()
    {
        switch (state)
        {
            case State.ready: Ready(); break;
            case State.decide: Decide(); break;
            case State.check: Check(); break;
            case State.attack: Attack(); break;
            case State.apply: Apply(); break;
            case State.end:
                break;
        }
        SetUI();
    }

    //Get Data
    static public State GetState()
    {
        return state;
    }

    static public void SetPlayer(GameObject player)
    {
        localPlayer = player;
    }

    static public PlayerInfo GetPlayerInfo()
    {
        return playerList[playerIndex];
    }

    static public void SetPlayerChoice(Choice choice)
    {
        PlayerInfo player = GetPlayerInfo();
        player.choice = choice;
    }

    static public void SetPlayerChallengeAmount(int amount)
    {
        PlayerInfo player = GetPlayerInfo();
        if (player.choice == Choice.challenge) player.challengeAmount = amount;
        else player.challengeAmount = 0;
        Debug.Log("player  : " + player.challengeAmount);
        Debug.Log("Gamble : " + GambleManager.GetPlayerInfo().challengeAmount);
    }

    public void AddPlayerCoins(int count)
    {
        GetPlayerInfo().coins += count;
    }

    void ResetPlayerInfoData()
    {
        foreach (var p in playerList)
        {
            p.choice = Choice.ready;
            p.challengeAmount = 0;
            p.canShoot = false;
        }
        victim = null;
    }

    int GetPotCoins(int round)
    {
        int weight = (int)Mathf.Pow(POT_WEIGHT, round - 1);
        int min = weight * 10;
        return Random.Range(min, min * (round + 1));
    }

    void SetChallengeWinner()
    {
        challengeWinner = null;

        var list = (from p in playerList
                    where p.choice == Choice.challenge && p.challengeAmount <= potCoins
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

    public void SetText(GameObject obj, string text)
    {
        obj.GetComponent<TextMeshProUGUI>().text = text;
    }

    void SetUI()
    {
        PlayerInfo player = GetPlayerInfo();
        SetText(playerInfoText.transform.Find("ChoiceText").gameObject, player.choice.ToString());
        SetText(playerInfoText.transform.Find("ChallengeAmountText").gameObject, player.challengeAmount.ToString());
        SetText(playerInfoText.transform.Find("CoinsText").gameObject, GetPlayerInfo().coins.ToString() + "G");


        SetText(gambleInfoText.transform.Find("RoundText").gameObject, "Round " + round.ToString());
        SetText(gambleInfoText.transform.Find("ActText").gameObject, "Act " + act.ToString());
        SetText(gambleInfoText.transform.Find("StateText").gameObject, state.ToString());
        SetText(gambleInfoText.transform.Find("PotCoinsText").gameObject, "Pot:" + potCoins.ToString());
    }


}