using UnityEngine;
using TMPro;

// 플레이어에 관련된 UI 로직을 수행하는 클래스
public class UIPlayerInfoScript : MonoBehaviour
{
    // 플레이어 랭크, 이름, 스코어를 표시하는 TextMeshProUGUI 요소들
    public TextMeshProUGUI playerRank;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerScore;

    // 플레이어 정보를 UI에 설정하는 함수
    public void SetTextUI(string rank, string name, string score)
    {
        // 주어진 정보로 TextMeshProUGUI 요소들을 업데이트합니다.
        playerRank.text = rank;
        playerName.text = name;
        playerScore.text = score;
    }
}
