using UnityEngine;

// VR 매핑 클래스
[System.Serializable]
public class VRMap
{
    // VR 타겟의 위치 및 회전을 추적할 대상
    public Transform vrTarget;

    // 리그 타겟의 위치 및 회전을 설정할 대상
    public Transform rigTarget;

    // 추적 위치의 오프셋
    public Vector3 trackingPositionOffset;

    // 추적 회전의 오프셋
    public Vector3 trackingRotationOffset;

    // VR 타겟의 위치와 회전을 리그 타겟에 매핑하는 함수
    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

// VR 리그 클래스
public class VRRig : MonoBehaviour
{
    // VR 매핑 객체
    public VRMap head;

    // 헤드 제약 조건을 나타내는 트랜스폼
    public Transform headConstraint;

    // 헤드와 몸의 오프셋
    public Vector3 headBodyOffset;

    // 회전 부드러움 정도
    public float turnSmoothness;

    // 시작 시 호출되는 함수
    private void Start()
    {
        // 헤드와 몸의 오프셋 계산
        headBodyOffset = transform.position - headConstraint.position;
    }

    // 고정 주기로 호출되는 함수
    private void FixedUpdate()
    {
        // 헤드의 위치 업데이트
        transform.position = headConstraint.position + headBodyOffset;

        // 헤드의 방향 업데이트 (부드러운 회전 적용)
        transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(-headConstraint.up, -Vector3.up).normalized,
            Time.deltaTime * turnSmoothness);

        // VR 매핑 함수 호출
        head.Map();
    }
}
