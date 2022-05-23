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

    public void DecideChallengeWinners(int potCoins)
    {
        winner = null;
        List<PlayerInfo> challengers = players.FindAll(player => player.IsChallengeSuccess(potCoins));
        if (challengers.Count == 0) return;

        int winnerAmount = challengers.Max(player => player.challengeAmount);
        List<PlayerInfo> winners = challengers.FindAll(player => player.challengeAmount == winnerAmount);
        if (winners.Count != 1) return;
        winner = winners.First();
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
        player.SetChoice(choice);
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

    public string ToJson()
    {
        return JsonUtility.ToJson(new Serialization<PlayerInfo>(players));
    }

    public static PlayerInfoList FromJson(string jdata)
    {
        List<PlayerInfo> players = JsonUtility.FromJson<Serialization<PlayerInfo>>(jdata).target;
        return new PlayerInfoList(players);
    }
}
