using System;

// 플레이어 정보를 저장하고 관리하는 클래스
// 게임에서 각 플레이어의 상태 및 행동을 나타냄
[Serializable]
public class PlayerInfo
{
    // 플레이어가 살아있는지 여부
    public bool isLive;

    // 보유 중인 동전의 양
    public int coins;

    // 도전할 때의 돈의 양
    public int challengeAmount;

    // 현재 플레이어가 공격할 수 있는지 여부
    public bool canShoot;

    // 플레이어의 번호
    public int actorNumber;

    // 플레이어의 이름
    public string name = "player";

    // 현재 라운드에서 획득한 돈
    public int winnings = 0;

    // 플레이어의 선택
    private Choice choice;

    // 남은 공격 기회
    private int attackChance;

    // 생성자: 플레이어 번호를 받아 초기화
    public PlayerInfo(int _actorNumber)
    {
        actorNumber = _actorNumber;
        name += _actorNumber.ToString();
        Reset();
        isLive = true;
        coins = 0;
        attackChance = 1;
    }

    // 플레이어 정보 초기화
    public void Reset()
    {
        choice = Choice.none;
        challengeAmount = 0;
        canShoot = false;
        winnings = 0;
    }

    // 선택 성공 시 발동하는 메서드
    public void SuccessChoiceAttack()
    {
        canShoot = true;
    }

    // 공격 메서드
    public void Attack(PlayerInfo target)
    {
        attackChance--;
        coins += target.coins;
        target.Dead();
        canShoot = false;
    }

    // 동전 추가 메서드
    public void AddCoin(int _coins)
    {
        coins += _coins;
        winnings = _coins;
    }

    // 도전 성공 시 호출되는 메서드
    public void ChallengeWin()
    {
        AddCoin(challengeAmount);
    }

    // 플레이어 사망 메서드
    public void Dead()
    {
        isLive = false;
        choice = Choice.none;
    }

    // 선택을 공유로 지정하는 메서드
    public void ChoiceShare()
    {
        choice = Choice.share;
    }

    // 도전 성공 여부 확인 메서드
    public bool IsChallengeSuccess(int potCoins)
    {
        return IsChallenge() && challengeAmount <= potCoins;
    }

    // 선택이 완료되었는지 확인하는 메서드
    public bool IsDecided()
    {
        return choice != Choice.none;
    }

    // 도전을 선택했는지 확인하는 메서드
    public bool IsChallenge()
    {
        return choice == Choice.challenge;
    }

    // 공격을 선택했는지 확인하는 메서드
    public bool IsAttack()
    {
        return choice == Choice.attack;
    }

    // 공유를 선택했는지 확인하는 메서드
    public bool IsShare()
    {
        return choice == Choice.share;
    }

    // 선택을 결정하는 메서드
    public void DecideChoice(Choice _choice)
    {
        choice = _choice;
    }

    // 도전 선택을 결정하는 메서드
    public void DecideChallenge(int amount)
    {
        choice = Choice.challenge;
        challengeAmount = amount;
    }
}

