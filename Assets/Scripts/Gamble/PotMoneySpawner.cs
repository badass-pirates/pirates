using UnityEngine;

// 보물상자에 대한 스폰 로직을 수행하는 클래스입니다.
public class PotMoneySpawner : MonoBehaviour
{
    // 1 라운드에 사용할 돈 포트 프리팹
    public GameObject oneRoundChest;

    // 2 라운드에 사용할 돈 포트 프리팹
    public GameObject twoRoundChest;

    // 3 라운드에 사용할 돈 포트 프리팹
    public GameObject threeRoundChest;

    // 생성된 돈 포트를 참조할 변수
    private GameObject chest;

    /// <summary>
    /// 주어진 라운드에 따라 돈 포트 생성 및 플레이어 쪽으로 회전시킵니다.
    /// </summary>
    /// <param name="playerTransform">포트가 바라볼 플레이어의 Transform</param>
    /// <param name="round">현재 라운드</param>
    public void SpawnPot(Transform playerTransform, int round)
    {
        // 라운드에 따라 다른 종류의 돈 포트를 생성합니다.
        if (round <= 1)
            chest = Instantiate(oneRoundChest, transform.position, Quaternion.identity);
        else if (round <= 2)
            chest = Instantiate(twoRoundChest, transform.position, Quaternion.identity);
        else if (round <= 3)
            chest = Instantiate(threeRoundChest, transform.position, Quaternion.identity);

        // 돈 포트를 현재 객체의 자식으로 설정하고 플레이어 쪽으로 회전시킵니다.
        chest.transform.parent = transform;
        chest.transform.LookAt(playerTransform);
    }

    /// <summary>
    /// 돈 포트를 파괴합니다.
    /// </summary>
    public void DestroyPot()
    {
        if (chest == null) return;
        else
        {
            // TODO: 삭제 애니메이션 적용 예정
            Destroy(chest);
            chest = null;
        }
    }
}
