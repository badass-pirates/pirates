using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils; // XROrigin 타입을 사용하기 위함

public class ContinuousMovement : MonoBehaviour
{
    public float speed = 1;
    public XRNode inputSource;
    public float gravity = -9.81f;
    public LayerMask groundLayer;
    public float additionalHeight = 0.2f;

    private float fallingSpeed;
    private XROrigin origin;
    private Vector2 inputAxis;
    private CharacterController character;

    void Start()
    {
        character = GetComponent<CharacterController>();
        origin = GetComponent<XROrigin>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0, origin.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        character.Move(direction * Time.fixedDeltaTime * speed);

        // gravity
        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    // Player 콜라이더가 유연하게 움직이도록 하기 위함 -> 틈 사이 들어가기
    // Character Controller의 Colider를 헤드셋에 매핑되어 유연하게 움직이도록 함. 
    void CapsuleFollowHeadset()
    {
        character.height = origin.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(origin.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }

    bool CheckIfGrounded()
    {
        // tells us if on ground 
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
