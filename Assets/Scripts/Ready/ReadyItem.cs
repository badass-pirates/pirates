using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static ReadyManager; // ReadyManager 클래스의 정적 멤버에 액세스하기 위한 using 선언
using Photon.Pun; // Photon Pun 라이브러리 사용
using UnityEngine.UI;

public class ReadyItem : XRGrabNetworkInteractable
{
    // 준비 아이템의 타이머 지속 시간
    public float timer;

    // 타이머를 표시할 캔버스
    public GameObject skullCanvas;

    // 타이머를 표시할 텍스트
    public Text countText;

    // 남은 타이머 시간 초기값
    private float leftTime = 9999;

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        // 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.IsMasterClient) return;

        // 아이템이 선택되지 않았으면 함수 종료
        if (!isSelected) return;

        // 스컬 캔버스가 활성화되어 있으면 타이머 텍스트 업데이트
        if (skullCanvas.activeSelf == true) countText.text = ((int)leftTime).ToString();

        // 남은 타이머가 0보다 작으면 씬 전환 함수 호출 후 함수 종료
        if (leftTime < 0)
        {
            RM.ChangeScene();
            return;
        }

        // 남은 타이머 시간 업데이트
        leftTime -= Time.deltaTime;
    }

    // 타이머 설정 함수
    public void SetTimer(float _timer)
    {
        timer = _timer;
    }

    // 아이템이 선택되었을 때 호출되는 함수
    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);
        // 남은 타이머 시간 초기화
        leftTime = timer;
    }

    // 아이템 선택이 해제될 때 호출되는 함수
    [System.Obsolete]
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        base.OnSelectExiting(interactor);
        // 남은 타이머 시간 초기화
        leftTime = timer;
    }
}
