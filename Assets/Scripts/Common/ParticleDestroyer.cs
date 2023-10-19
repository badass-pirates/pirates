using UnityEngine;
using Photon.Pun;

// 파티클이 정지되면 해당 게임 오브젝트를 제거하는 스크립트
public class ParticleDestroyer : MonoBehaviour
{
    // 파티클 시스템이 정지되었을 때 호출되는 콜백
    private void OnParticleSystemStopped()
    {
        // 현재 PhotonView가 로컬 플레이어의 것인지 확인
        if (this.GetComponent<PhotonView>().IsMine)
        {
            // 게임 오브젝트를 네트워크 상에서 제거
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
