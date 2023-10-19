using Photon.Pun;
using UnityEngine;

// 총에 관련된 기능을 처리하는 클래스
public class GunManager : MonoBehaviour
{
    // 라인 렌더러 및 이펙트에 대한 참조 변수 선언
    public LineRenderer lineRenderer;
    public GameObject gunFx, bloodFx, dustFx;

    // 총의 총구와 조준 지점에 대한 참조 변수 선언
    public GameObject barrel, aimingPoint;

    private RaycastHit hit;
    private bool isHit = false;
    private bool isGrab = false;

    // 매 프레임마다 실행되는 업데이트 메서드
    void Update()
    {
        // 라인 그리기 실행
        DrawLine();
    }

    // 총을 잡았는지 확인하는 메서드
    public void CheckGrab(bool isGrab)
    {
        // 총을 잡았을 때 총구 활성화
        this.isGrab = isGrab;
        barrel.SetActive(isGrab);
    }

    // 라인 그리기 메서드
    void DrawLine()
    {
        // 총구 방향 계산
        Vector3 direction = barrel.transform.forward;

        // Raycast를 사용하여 충돌 여부 확인
        isHit = Physics.Raycast(barrel.transform.position, direction, out hit);

        // 충돌했다면 라인 그리기 및 조준 지점 이동
        if (isHit)
        {
            lineRenderer.SetPosition(0, barrel.transform.position);
            lineRenderer.SetPosition(1, hit.point);
            aimingPoint.transform.position = hit.point;

            // 자기 자신에게 충돌했다면 무시
            if (hit.transform == transform)
            {
                isHit = false;
                return;
            }
        }
    }

    // 발사 메서드
    public void Shoot()
    {
        int targetActorNumber = -1;

        // 총을 잡은 상태가 아니면 발사하지 않음
        if (!isGrab)
            return;

        // 라인 그리기 실행
        DrawLine();

        // 라인이 충돌하지 않았으면 발사하지 않음
        if (!isHit)
            return;

        // 충돌한 대상이 플레이어인지 확인
        GameObject target = hit.transform.gameObject;
        bool isPlayer = target.layer == 9; // 9는 Body 레이어
        if (isPlayer)
        {
            // 충돌 대상이 플레이어인 경우 해당 플레이어의 PhotonView를 통해 ActorNumber 확인
            target = target.GetComponentInParent<NetworkPlayer>().gameObject;
            PhotonView pv = target.GetComponent<PhotonView>();
            targetActorNumber = pv ? pv.ViewID / 1000 : -1;
        }

        // 피격 이펙트 재생 및 공격 실행
        PlayHitEffect(isPlayer, aimingPoint.transform);
        GambleManager.Attack(targetActorNumber);
        PhotonNetwork.Destroy(gameObject);
    }

    // 피격 이펙트 재생 메서드
    void PlayHitEffect(bool isPlayer, Transform tr)
    {
        // 총 발사 이펙트 및 플레이어 또는 더스트 이펙트 재생
        PhotonNetwork.Instantiate(gunFx.name, barrel.transform.position, barrel.transform.rotation);
        PhotonNetwork.Instantiate(isPlayer ? bloodFx.name : dustFx.name, tr.position, tr.rotation);
    }
}
