using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

// 도전 금액을 조절하는 클래스
public class ChallengeAmount : MonoBehaviour
{
    // 도전 금액을 표시하는 캔버스
    public GameObject challengeCanvas;

    // 도전 금액을 표시하는 왼쪽 이미지
    public Image amountLeft;

    // 도전 금액을 표시하는 오른쪽 이미지
    public Image amountRight;

    // 도전 금액을 표시하는 텍스트
    public Text amountText;

    // 도전 금액의 속성
    public float amount { get; private set; }

    // XR 컨트롤러
    private XRController controller;

    // 도전 금액 변경 속도
    float velocity = 1;

    // 시작 시 호출되는 함수
    void Start()
    {
        // XR 컨트롤러 초기화
        controller = GameObject.Find("RightHand Controller").GetComponent<XRController>();

        // 도전 최소 금액 설정
        amount = GambleManager.GetMinPotCoins();
    }

    // 프레임마다 호출되는 함수
    void Update()
    {
        // 도전 캔버스가 활성화된 경우에만 실행
        if (challengeCanvas.activeSelf == true)
        {
            // 도전 금액 결정 및 표시 업데이트
            DecideAmount();
            amountText.text = ((int)amount).ToString();
        }
    }

    // 도전 금액을 결정하는 함수
    private void DecideAmount()
    {
        // XR 컨트롤러의 주 터치 패드 값을 가져옴
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
        {
            // 속도 증가
            velocity *= 1.05f;

            // 오른쪽으로 이동하면서 최대 도전 금액 이하일 때
            if (position.x > 0 && amount < GambleManager.GetMaxPotCoins())
            {
                amount += Time.deltaTime * velocity;
            }
            // 왼쪽으로 이동하면서 최소 도전 금액 초과일 때
            else if (position.x < 0 && (int)amount > GambleManager.GetMinPotCoins())
            {
                amount -= Time.deltaTime * velocity;
            }
            // 이동 없을 때
            else
            {
                // 속도 초기화 및 금액 올림 처리
                velocity = 1;
                Mathf.Ceil(amount);
            }
        }
    }
}
