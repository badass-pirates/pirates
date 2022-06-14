using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class PlayerInfoList
{
    private List<PlayerInfo> players = new List<PlayerInfo>();
    private PlayerInfo winner = null;
    private PlayerInfo attacker = null;

    public PlayerInfoList() { }

    public PlayerInfoList(List<PlayerInfo> _players)
    {
        players = _players;
    }
    public PlayerInfoList(List<PlayerInfo> _players, PlayerInfo _winner = null, PlayerInfo _attacker = null)
    {
        players = _players;
        winner = _winner;
        attacker = _attacker;
    }

    public void Add(int actorNumber)
    {
        players.Add(new PlayerInfo(actorNumber));
    }

    public PlayerInfo Find(int actorNumber)
    {
        return players.Find(player => player.actorNumber == actorNumber);
    }

    public void Reset()
    {
        players.ForEach(player => player.Reset());
    }

    public bool EveryDecided()
    {
        return players.TrueForAll(player => player.IsDecided());
    }

    public void ChangeUndecidedPlayerToShare()
    {
        players.FindAll(player => player.isLive && !player.IsDecided())
            .ForEach(player => player.ChoiceShare());
    }

    public void DecideChallengeWinner(int potCoins)
    {
        winner = null;
        List<PlayerInfo> challengers = players.FindAll(player => player.IsChallengeSuccess(potCoins));
        if (challengers.Count == 0) return;

        int winnerAmount = challengers.Max(player => player.challengeAmount);
        List<PlayerInfo> winners = challengers.FindAll(player => player.challengeAmount == winnerAmount);
        if (winners.Count != 1) return;
        winner = winners.First();
        Debug.Log("OnCheck/ ME : " + PhotonNetwork.LocalPlayer.ActorNumber + "| Winner : " + winner.actorNumber + "| Amount : " + winner.challengeAmount + "/" + winnerAmount);
    }

    public void DecideAttackWinner()
    {
        attacker = null;
        List<PlayerInfo> attackers = players.FindAll(player => player.IsAttack());
        if (attackers.Count != 1) return;

        attacker = attackers.First();
    }

    public void ShareCoins(int potCoins)
    {
        List<PlayerInfo> sharer = players.FindAll(player => player.IsShare());
        if (sharer.Count == 0) return;

        int sharedCoins = potCoins / sharer.Count;
        sharer.ForEach(player => player.AddCoin(sharedCoins));
    }

    public void SetPlayerChoice(int actorNumber, Choice choice)
    {
        PlayerInfo player = Find(actorNumber);
        player.DecideChoice(choice);
    }

    public void DecidePlayerChallenge(int actorNumber, int amount)
    {
        PlayerInfo player = Find(actorNumber);
        player.DecideChallenge(amount);
    }

    public PlayerInfo GetMine()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        return players.Find(player => player.actorNumber == actorNumber);
    }

    public int Count()
    {
        return players.Count;
    }

    public PlayerInfo GetChallengeWinner()
    {
        return winner;
    }

    public PlayerInfo GetAttackWinner()
    {
        return attacker;
    }

    public List<PlayerInfo> GetRankList()
    {
        List<PlayerInfo> rankList = new List<PlayerInfo>(players);
        rankList.Sort(delegate (PlayerInfo A, PlayerInfo B)
        {
            if (A.coins < B.coins) return -1;
            else if (A.coins > B.coins) return 1;
            else
            {
                if (A.actorNumber < B.actorNumber) return -1;
                else if (A.actorNumber > B.actorNumber) return 1;
            }
            return 0;
        });
        return rankList;
    }

    public (string, string, string) ToJson()
    {
        string jsonPlayers = JsonUtility.ToJson(new Serialization<PlayerInfo>(players));
        string jsonWinner = JsonUtility.ToJson(winner);
        string jsonAttacker = JsonUtility.ToJson(attacker);
        return (jsonPlayers, jsonWinner, jsonAttacker);
    }

    public static PlayerInfoList FromJson(string _players, string _winner = null, string _attacker = null)
    {
        List<PlayerInfo> players = JsonUtility.FromJson<Serialization<PlayerInfo>>(_players).ToList();
        PlayerInfo winner = JsonUtility.FromJson<PlayerInfo>(_winner);
        PlayerInfo attacker = JsonUtility.FromJson<PlayerInfo>(_attacker);
        return new PlayerInfoList(players, winner, attacker);
    }
}
