using UnityEngine;
using Photon.Pun;

// VR 플레이어의 네트워크 리그 스크립트
[System.Serializable]
public class VRNetworkPlayerRig : MonoBehaviourPunCallbacks
{
    // VR 헤드 모델의 트랜스폼
    private Transform vrHeadModel;

    // 리그 헤드 모델의 트랜스폼
    public Transform rigHeadModel;

    // VR 헤드 매핑을 담당하는 VRMap 객체
    public VRMap head;

    // Photon 네트워크 뷰
    public PhotonView PV;

    // VR 플레이어 게임 오브젝트
    public GameObject vrPlayer;

    // 헤드의 제약 조건을 나타내는 트랜스폼
    public Transform headConstraint;

    // 헤드와 몸의 오프셋
    public Vector3 headBodyOffset;

    // 회전 부드러움 정도
    public float turnSmoothness = 5f;

    // 시작 시 호출되는 함수
    private void Start()
    {
        // 현재 Photon 뷰가 로컬 플레이어에 속해있는 경우
        if (PV.IsMine)
        {
            // 헤드와 몸의 오프셋 계산
            headBodyOffset = transform.position - headConstraint.position;

            // VR 타겟과 헤드 모델 탐색
            head.vrTarget = GameObject.Find("Player").transform.Find("Camera Offset/Main Camera");
            vrHeadModel = GameObject.Find("Player").transform.Find("Camera Offset/Model/Root/Hips/Spine_01/Spine_02/Spine_03/Neck/Head");
        }
    }

    // 매 프레임마다 호출되는 함수
    private void Update()
    {
        // 현재 Photon 뷰가 로컬 플레이어에 속해있는 경우
        if (PV.IsMine)
        {
            // VR 플레이어 비활성화
            vrPlayer.SetActive(false);

            // 헤드의 위치 업데이트
            transform.position = headConstraint.position + headBodyOffset;

            // 헤드의 방향 업데이트 (부드러운 회전 적용)
            transform.forward = Vector3.Lerp(transform.forward,
                Vector3.ProjectOnPlane(-headConstraint.up, -Vector3.up).normalized, Time.deltaTime * turnSmoothness);

            // VR 매핑 함수 호출
            head.Map();

            // 리그 헤드 모델의 스케일 조정
            rigHeadModel.localScale = vrHeadModel.localScale;
        }
    }
}
