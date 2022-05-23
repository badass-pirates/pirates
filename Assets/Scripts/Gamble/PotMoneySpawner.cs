using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotMoneySpawner : MonoBehaviour
{
    public GameObject oneRoundChest;
    public GameObject twoRoundChest;
    public GameObject threeRoundChest;
    private GameObject chest;

    public void SpawnPot(Transform playerTransform, int round)
    {
        if (round == 1) chest = Instantiate(oneRoundChest, transform.position, Quaternion.identity);
        else if (round == 2) chest = Instantiate(twoRoundChest, transform.position, Quaternion.identity);
        else if (round == 3) chest = Instantiate(threeRoundChest, transform.position, Quaternion.identity);
        chest.transform.parent = transform;
        chest.transform.LookAt(playerTransform);
    }

    public void DistroyPot()
    {
        if (chest == null) return;
        else
        {
            // 삭제 애니메이션 적용 예정
            Destroy(chest);
        }
    }
}
