using UnityEngine;
using TMPro;

// UI 관련 로직을 수행하는 클래스
public class UIManager : MonoBehaviour
{
    // 게임 정보를 표시하는 UI 객체와 게임 종료 시 활성화되는 UI 객체 선언
    public GameObject gambleInfo;
    public GameObject endingUI;

    // UI에서 사용할 텍스트 요소들
    private TextMeshProUGUI round, leftTime, potRange, myCoins;

    // 게임 종료 UI가 바라보는 위치를 저장하는 변수
    private Vector3 targetPosition;

    void Start()
    {
        // UI 요소들을 초기화합니다.
        round = gambleInfo.transform.Find("RoundText").GetComponent<TextMeshProUGUI>();
        leftTime = gambleInfo.transform.Find("LeftTimeText").GetComponent<TextMeshProUGUI>();
        potRange = gambleInfo.transform.Find("PotRangeText").GetComponent<TextMeshProUGUI>();
        myCoins = gambleInfo.transform.Find("MyCoinsText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // 로컬 플레이어가 존재하면 UI 텍스트를 업데이트합니다.
        if (GambleManager.localPlayer != null)
        {
            SetTextUI();
        }
    }

    // UI 텍스트를 업데이트하는 함수
    void SetTextUI()
    {
        // 현재 로컬 플레이어의 정보를 가져옵니다.
        PlayerInfo player = GambleManager.GetMyInfo();
        if (player == null) return;

        // UI에 관련 정보를 설정합니다.
        gambleInfo.transform.LookAt(GambleManager.localPlayer.transform);
        gambleInfo.transform.Rotate(0, 180, 0);
        round.text = GambleManager.round + "-" + GambleManager.act;
        leftTime.text = ((int)GambleManager.leftTime).ToString();
        potRange.text = "[" + GambleManager.GetMinPotCoins() + " ~ " + GambleManager.GetMaxPotCoins() + "]";
        myCoins.text = " My : " + player.coins + "G";
    }

    // 게임 종료 시 UI를 설정하는 함수
    public void SetEndingTextUI()
    {
        // 게임 종료 UI 활성화 및 위치 설정
        gambleInfo.SetActive(false);
        endingUI.SetActive(true);
        targetPosition = new Vector3(GambleManager.localPlayer.transform.position.x, endingUI.transform.position.y, GambleManager.localPlayer.transform.position.z);
        endingUI.transform.LookAt(targetPosition);
        endingUI.transform.Translate(new Vector3(0, 0, 1));
    }
}
