using UnityEngine;

// 엔딩 화면의 UI 콘텐츠를 관리하는 클래스
public class EndingUIContents : MonoBehaviour
{
    // 플레이어 이름 배열
    private string[] playerName;

    // 플레이어 점수 배열
    private int[] playerScore;

    // 플레이어 정보를 표시하는 UI 아이템 프리팹
    public GameObject playerInfoItem;

    // 다른 플레이어 정보를 표시하는 UI 아이템 프리팹
    public GameObject otherInfoItem;

    // 순위를 나타내는 변수
    private int rank = 1;

    // 시작 시 호출되는 함수
    void Start()
    {
        // 초기화 함수 호출
        Init();
    }

    // 초기화 함수
    private void Init()
    {
        // 모든 플레이어의 정보를 가져와 순위별로 UI에 추가
        foreach (PlayerInfo player in GambleManager.players.GetRankList())
        {
            // 자신의 정보인 경우
            if (GambleManager.GetMyInfo().name == player.name)
            {
                // 플레이어 정보 아이템 생성 및 UI 설정
                GameObject playerItem = Instantiate(playerInfoItem);
                playerItem.GetComponent<UIPlayerInfoScript>().SetTextUI(rank.ToString(), player.name, player.coins.ToString());
                playerItem.transform.SetParent(transform, false);
            }
            // 다른 플레이어의 정보인 경우
            else
            {
                // 다른 플레이어 정보 아이템 생성 및 UI 설정
                GameObject otherItem = Instantiate(otherInfoItem);
                otherItem.GetComponent<UIPlayerInfoScript>().SetTextUI(rank.ToString(), player.name, player.coins.ToString());
                otherItem.transform.SetParent(transform, false);
            }

            // 순위 증가
            rank++;
        }
    }
}
