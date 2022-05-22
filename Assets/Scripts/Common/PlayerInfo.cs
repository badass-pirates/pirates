public class PlayerInfo
{
    public bool isLive { get; private set; }
    public int coins { get; private set; }
    public Choice choice { get; private set; }
    public int challengeAmount { get; private set; }
    public bool canShoot { get; private set; }
    public int attackChance { get; private set; }

    public int actorNumber { get; private set; }
    
    public PlayerInfo()
    {
        actorNumber = -1;
        Reset();
    }

    public PlayerInfo(int _actorNumber)
    {
        actorNumber = _actorNumber;
        Reset();
    }

    public void Reset()
    {
        isLive = true;
        coins = 0;
        choice = Choice.none;
        challengeAmount = 0;
        canShoot = false;
        attackChance = 1;
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
    }

    public void AddCoin(int _coins)
    {
        coins += _coins;
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

    public bool IsDecide()
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

    public void SetChoice(Choice _choice)
    {
        choice = _choice;
    }

    public void SetChallengeAmount(int amount)
    {
        challengeAmount = amount;
    }
}
