using UnityEngine;
using Photon.Pun;
using static Utils;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    // PhotonView 컴포넌트에 대한 참조
    public PhotonView PV;

    // 준비 아이템이 생성될 위치에 대한 참조
    public Transform readyItemSpawnPoint;

    // 준비 타이머의 지속 시간
    public float timer;

    // 현재 생성된 준비 아이템에 대한 참조
    private GameObject readyItem;

    #region Singleton
    public static ReadyManager RM; // 싱글톤 인스턴스
    void Awake()
    {
        if (RM == null)
        {
            RM = this; // 현재 인스턴스를 싱글톤으로 설정
        }
        else Destroy(this); // 이미 다른 인스턴스가 존재하면 현재 인스턴스 파괴
    }
    #endregion

    // 다른 씬으로 전환하는 함수
    public void ChangeScene()
    {
        // 마스터 클라이언트가 아니면 함수 종료
        if (!PhotonNetwork.IsMasterClient) return;

        // 현재 준비 아이템의 트랜스폼 정보 저장
        Transform transform = readyItem.transform;

        // 현재 준비 아이템 제거
        PhotonNetwork.Destroy(readyItem);
        PhotonNetwork.Instantiate("Ready/SkullExplosion", transform.position, transform.rotation); // 폭발 이펙트 생성

        // 씬 전환을 지연시키기 위해 코루틴 실행
        StartCoroutine(U.ChangeScene("GambleScene", 1));
    }

    // 플레이어가 방에 입장할 때 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        // 기본 콜백 함수 호출
        base.OnJoinedRoom();

        // PhotonView가 현재 플레이어의 것이 아니면 함수 종료
        if (!PV.IsMine)
        {
            return;
        }

        // 준비 아이템 생성 및 설정
        readyItem = PhotonNetwork.Instantiate("Ready/Skull", readyItemSpawnPoint.position, readyItemSpawnPoint.rotation);
        ReadyItem item = readyItem.GetComponent<ReadyItem>();

        // 타이머 설정
        item.SetTimer(timer);
    }
}
