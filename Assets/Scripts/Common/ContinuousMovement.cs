using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

// VR 환경에서 지속적인 이동을 처리하는 스크립트
// Unity XR 패키지를 사용하여 VR 입력 및 공간 제어
public class ContinuousMovement : MonoBehaviour
{
    // 이동 속도
    public float speed = 1;

    // 사용자 입력 소스 (e.g., 왼손 컨트롤러)
    public XRNode inputSource;

    // 중력
    public float gravity = -9.81f;

    // 땅으로 간주할 레이어
    public LayerMask groundLayer;

    // 캡슐 콜라이더의 추가 높이
    public float additionalHeight = 0.2f;

    // 떨어지는 속도
    private float fallingSpeed;

    // XR 공간의 원점 (헤드셋의 위치 및 방향 제어)
    private XROrigin origin;

    // 사용자 입력의 2D 축 값
    private Vector2 inputAxis;

    // 캐릭터 컨트롤러 컴포넌트
    private CharacterController character;

    void Start()
    {
        // 캐릭터 컨트롤러 컴포넌트 참조
        character = GetComponent<CharacterController>();

        // XR 공간의 원점 컴포넌트 참조
        origin = GetComponent<XROrigin>();
    }

    void Update()
    {
        // 사용자 입력 장치에서 2D 축 값을 가져옴
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        // 헤드셋의 위치에 따라 캡슐 콜라이더를 따라 이동
        CapsuleFollowHeadset();

        // 사용자 입력에 기반하여 이동 방향을 계산
        Quaternion headYaw = Quaternion.Euler(0, origin.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        // 캐릭터를 이동 방향으로 움직임
        character.Move(direction * Time.fixedDeltaTime * speed);

        // 중력 처리
        bool isGrounded = CheckIfGrounded(); // 땅에 닿았는지 확인
        if (isGrounded)
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        // 캐릭터를 위아래로 움직여 중력 적용
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    // 캡슐 콜라이더를 헤드셋에 따라 유연하게 이동시키는 메서드
    void CapsuleFollowHeadset()
    {
        character.height = origin.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(origin.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    // 땅에 닿았는지 확인하는 메서드
    bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
