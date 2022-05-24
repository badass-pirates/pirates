using System;
using UnityEngine;

[Serializable]
public class PlayerInfo
{
    [SerializeField]
    public bool isLive;
    [SerializeField]
    public int coins;
    [SerializeField]
    public Choice choice;
    [SerializeField]
    public int challengeAmount;
    [SerializeField]
    public bool canShoot;
    [SerializeField]
    public int attackChance;
    [SerializeField]
    public int actorNumber;

    public PlayerInfo(int _actorNumber)
    {
        actorNumber = _actorNumber;
        Reset();
        isLive = true;
        coins = 0;
        attackChance = 1;
    }

    public void Reset()
    {
        choice = Choice.none;
        challengeAmount = 0;
        canShoot = false;
    }

    public void SuccessChoiceAttack()
    {
        canShoot = true;
    }

    public void Attack(PlayerInfo target)
    {
        attackChance--;
        coins += target.coins;
        target.Dead();
        canShoot = false;
    }

    public void AddCoin(int _coins)
    {
        coins += _coins;
    }

    public void ChallengeWin()
    {
        AddCoin(challengeAmount);
    }

    public void Dead()
    {
        isLive = false;
        choice = Choice.none;
    }

    public void ChoiceShare()
    {
        choice = Choice.share;
    }

    public bool IsChallengeSuccess(int potCoins)
    {
        return IsChallenge() && challengeAmount <= potCoins; 
    }

    public bool IsDecided()
    {
        return choice != Choice.none;
    }

    public bool IsChallenge()
    {
        return choice == Choice.challenge;
    }

    public bool IsAttack()
    {
        return choice == Choice.attack;
    }

    public bool IsShare()
    {
        return choice == Choice.share;
    }

    public void DecideChoice(Choice _choice)
    {
        choice = _choice;
    }

    public void DecideChallenge(int amount)
    {
        choice = Choice.challenge;
        challengeAmount = amount;
    }
}
