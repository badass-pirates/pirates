using Photon.Pun;
using UnityEngine;

// 메달에 대한 로직을 수행하는 클래스
public class Medal : MonoBehaviour
{
    // 선택한 옵션
    public Choice choice;

    // 로컬 플레이어에 대한 참조
    public GameObject localPlayer;

    // 사라지는 효과와 선택된 효과에 사용되는 파티클 시스템
    public ParticleSystem disappearEffect, choseEffect;

    // 플레이어와 옵션을 선택할 영역을 나타내는 게임 오브젝트
    GameObject playerZone, choiceZone;

    // 올바른 위치에 있는지 여부
    bool isCorrectPos;

    // 위치 확인 제한 시간
    const float timeConstraint = 2f;

    // 흐른 시간
    float passTime = 0f;

    void Awake()
    {
        // 게임 오브젝트를 찾아서 참조
        playerZone = GameObject.Find("PlayerZone").gameObject;
        choiceZone = GameObject.Find("ChoiceZone").gameObject;

        // PhotonView를 사용하여 네트워크 오브젝트인 경우에만 스크립트 유지
        if (!gameObject.GetComponent<PhotonView>().IsMine) Destroy(this);
    }

    void Update()
    {
        // 올바른 위치에 있지 않은 경우 흐른 시간 증가
        if (!isCorrectPos)
            passTime += Time.deltaTime;

        // 위치 확인 제한 시간을 초과한 경우, 로컬 플레이어의 메달 재생성 및 시간 초기화
        if (passTime >= timeConstraint)
        {
            GambleManager.localPlayer.ReSpawnMedals();
            passTime = 0f;
        }
    }

    // 플레이어나 옵션 영역에 진입했을 때 호출되는 이벤트
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 영역에 진입한 경우
        if (other.gameObject == playerZone)
        {
            isCorrectPos = true;
            passTime = 0f;
        }
        // 옵션 영역에 진입한 경우
        else if (other.gameObject == choiceZone)
        {
            isCorrectPos = true;
            passTime = 0f;

            // 만약 선택한 옵션이 도전(Challenge)인 경우
            if (choice == Choice.challenge)
            {
                ChallengeAmount cAmount = GetComponent<ChallengeAmount>();
                // 도전의 양을 가져와서 플레이어의 도전 결정 메서드 호출
                GambleManager.DecidePlayerChallenge((int)cAmount.amount);
                return;
            }

            // 그 외의 경우, 선택한 옵션을 결정하는 메서드 호출
            GambleManager.DecideChoice(choice);
        }
    }

    // 플레이어나 옵션 영역에서 나왔을 때 호출되는 이벤트
    private void OnTriggerExit(Collider other)
    {
        // 플레이어 영역이나 옵션 영역에서 나간 경우, 올바른 위치에 있지 않다고 설정
        if (other.gameObject == playerZone || other.gameObject == choiceZone)
            isCorrectPos = false;
    }
}
