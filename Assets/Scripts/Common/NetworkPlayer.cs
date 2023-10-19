using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using TMPro;

// 네트워크 플레이어를 제어하는 스크립트
public class NetworkPlayer : MonoBehaviour
{
    // 왼손 및 오른손 위치를 나타내는 Transform
    public Transform leftHand;
    public Transform rightHand;

    // 왼손 및 오른손의 애니메이터
    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    // 플레이어 이름을 표시하는 TextMeshProUGUI
    public TextMeshProUGUI playerNameText;

    // PhotonView 컴포넌트
    private PhotonView photonView;

    // 로컬 플레이어의 왼손 및 오른손 레이그 트랜스폼
    private Transform leftHandRig;
    private Transform rightHandRig;

    void Start()
    {
        // PhotonView 컴포넌트 초기화
        photonView = GetComponent<PhotonView>();

        // 플레이어 이름 설정
        playerNameText.text = "Player " + (photonView.ViewID / 1000).ToString();

        // 로컬 플레이어인 경우
        if (photonView.IsMine)
        {
            // 자신의 렌더러를 비활성화
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }

            // 로컬 플레이어의 왼손과 오른손 레이그 트랜스폼을 찾아서 할당
            leftHandRig = GameObject.Find("Player").transform.Find("Camera Offset/LeftHand Controller");
            rightHandRig = GameObject.Find("Player").transform.Find("Camera Offset/RightHand Controller");
        }
    }

    void Update()
    {
        // 로컬 플레이어인 경우에만 실행
        if (photonView.IsMine)
        {
            // 왼손과 오른손의 위치를 레이그 트랜스폼에 매핑
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);

            // 왼손과 오른손의 애니메이터 업데이트
            UpdateHandAnimator(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimator(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    // 타겟 트랜스폼에 레이그 트랜스폼의 위치 및 회전을 매핑하는 메서드
    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }

    // 손의 애니메이터를 업데이트하는 메서드
    void UpdateHandAnimator(InputDevice targetDevice, Animator handAnimator)
    {
        // 트리거 값을 읽어 애니메이터에 전달
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        // 그립(손잡이) 값을 읽어 애니메이터에 전달
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
}
