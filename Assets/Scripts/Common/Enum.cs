// 플레이어의 선택을 나타내는 열거형
public enum Choice
{
    none = 0,       // 선택 없음
    share = 1,      // 공유 선택
    challenge = 2,  // 도전 선택
    attack = 3      // 공격 선택
}

// 게임 상태를 나타내는 열거형
public enum State
{
    loading = -1,   // 로딩 중
    initial = 0,    // 초기 상태
    standBy = 1,    // 대기 상태
    decide = 2,     // 결정 상태
    check = 3,      // 확인 상태
    attack = 4,     // 공격 상태
    apply = 5,      // 적용 상태
    end = 6         // 종료 상태
}
