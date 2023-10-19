using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// 플레이어 리스트 정보를 관리하는 클래스
public class PlayerInfoList
{
    private List<PlayerInfo> players = new List<PlayerInfo>();
    private PlayerInfo winner = null;
    private PlayerInfo attacker = null;

    // 기본 생성자
    public PlayerInfoList() { }

    // 생성자 오버로딩
    public PlayerInfoList(List<PlayerInfo> _players)
    {
        players = _players;
    }

    // 생성자 오버로딩
    public PlayerInfoList(List<PlayerInfo> _players, PlayerInfo _winner = null, PlayerInfo _attacker = null)
    {
        players = _players;
        winner = _winner;
        attacker = _attacker;
    }

    // 새로운 플레이어 추가
    public void Add(int actorNumber)
    {
        players.Add(new PlayerInfo(actorNumber));
    }

    // 특정 actorNumber를 가진 플레이어 찾기
    public PlayerInfo Find(int actorNumber)
    {
        return players.Find(player => player.actorNumber == actorNumber);
    }

    // 플레이어 정보 초기화
    public void Reset()
    {
        players.ForEach(player => player.Reset());
    }

    // 모든 플레이어가 결정했는지 확인
    public bool EveryDecided()
    {
        return players.FindAll(player => player.isLive)
            .TrueForAll(player => player.IsDecided());
    }

    // 결정되지 않은 플레이어를 공유로 변경
    public void ChangeUndecidedPlayerToShare()
    {
        players.FindAll(player => player.isLive && !player.IsDecided())
            .ForEach(player => player.ChoiceShare());
    }

    // 도전자 승자 결정
    public void DecideChallengeWinner(int potCoins)
    {
        winner = null;
        List<PlayerInfo> challengers = players.FindAll(player => player.IsChallengeSuccess(potCoins));
        if (challengers.Count == 0) return;

        int winnerAmount = challengers.Max(player => player.challengeAmount);
        List<PlayerInfo> winners = challengers.FindAll(player => player.challengeAmount == winnerAmount);
        if (winners.Count != 1) return;

        winner = winners.First();
    }

    // 도전자 이름 목록 가져오기
    public List<string> GetChallengersName()
    {
        return players.FindAll(player => player.IsChallenge())
            .Select(player => player.name)
            .ToList<string>();
    }

    // 공격자 승자 결정
    public void DecideAttackWinner()
    {
        attacker = null;
        List<PlayerInfo> attackers = players.FindAll(player => player.IsAttack());
        if (attackers.Count != 1) return;

        attacker = attackers.First();
    }

    // 공격자 이름 목록 가져오기
    public List<string> GetAttackersName()
    {
        return players.FindAll(player => player.IsAttack())
            .Select(player => player.name)
            .ToList<string>();
    }

    // 돈을 나누어주는 메서드
    public void ShareCoins(int potCoins)
    {
        List<PlayerInfo> sharer = players.FindAll(player => player.IsShare());
        if (sharer.Count == 0) return;

        int sharedCoins = potCoins / sharer.Count;
        sharer.ForEach(player => player.AddCoin(sharedCoins));
    }

    // 특정 플레이어의 선택을 설정하는 메서드
    public void SetPlayerChoice(int actorNumber, Choice choice)
    {
        PlayerInfo player = Find(actorNumber);
        player.DecideChoice(choice);
    }

    // 특정 플레이어의 도전 결정을 설정하는 메서드
    public void DecidePlayerChallenge(int actorNumber, int amount)
    {
        PlayerInfo player = Find(actorNumber);
        player.DecideChallenge(amount);
    }

    // 로컬 플레이어의 정보 가져오기
    public PlayerInfo GetMine()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        return players.Find(player => player.actorNumber == actorNumber);
    }

    // 플레이어 수 반환
    public int Count()
    {
        return players.Count;
    }

    // 도전자 승자 정보 반환
    public PlayerInfo GetChallengeWinner()
    {
        return winner;
    }

    // 공격자 승자 정보 반환
    public PlayerInfo GetAttackWinner()
    {
        return attacker;
    }

    // 순위 목록 가져오기
    public List<PlayerInfo> GetRankList()
    {
        // 플레이어 정보를 복사하여 정렬
        List<PlayerInfo> rankList = new List<PlayerInfo>(players);
        rankList.Sort(delegate (PlayerInfo A, PlayerInfo B)
        {
            if (A.coins > B.coins) return -1;
            else if (A.coins < B.coins) return 1;
            else
            {
                if (A.actorNumber > B.actorNumber) return -1;
                else if (A.actorNumber < B.actorNumber) return 1;
            }
            return 0;
        });
        return rankList;
    }

    // JSON 직렬화 메서드
    public (string, string, string) ToJson()
    {
        string jsonPlayers = JsonUtility.ToJson(new Serialization<PlayerInfo>(players));
        string jsonWinner = JsonUtility.ToJson(winner);
        string jsonAttacker = JsonUtility.ToJson(attacker);
        return (jsonPlayers, jsonWinner, jsonAttacker);
    }

    // JSON 역직렬화 메서드
    public static PlayerInfoList FromJson(string _players, string _winner = null, string _attacker = null)
    {
        List<PlayerInfo> players = JsonUtility.FromJson<Serialization<PlayerInfo>>(_players).ToList();
        PlayerInfo winner = JsonUtility.FromJson<PlayerInfo>(_winner);
        PlayerInfo attacker = JsonUtility.FromJson<PlayerInfo>(_attacker);
        return new PlayerInfoList(players, winner, attacker);
    }
}
