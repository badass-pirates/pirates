using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

// XR 그랩 네트워크 상호작용 클래스
public class XRGrabNetworkInteractable : XRGrabInteractable
{
    // Photon 네트워크 뷰
    public PhotonView photonView;

    // 리지드바디 컴포넌트
    public Rigidbody rig;

    // 시작 시 호출되는 함수
    private void Start()
    {
        // Photon 뷰가 로컬 플레이어에 속해있지 않은 경우
        if (!photonView.IsMine)
        {
            // 중력 사용하지 않도록 설정
            rig.useGravity = false;

            // 컴포넌트 비활성화
            enabled = false;
        }
    }

    // 선택이 시작될 때 호출되는 함수
    [System.Obsolete]
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        // 소유권 요청
        photonView.RequestOwnership();

        // 기본 OnSelectEntered 함수 호출
        base.OnSelectEntered(interactor);
    }

    // 선택이 끝날 때 호출되는 함수
    [System.Obsolete]
    protected override void OnSelectExiting(XRBaseInteractor interactor)
    {
        // 기본 OnSelectExiting 함수 호출
        base.OnSelectExiting(interactor);
    }
}
