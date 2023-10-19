using UnityEngine;
using Photon.Pun;

// 플레이어에 대한 처리를 수행하는 클래스
public class GamblePlayer : MonoBehaviour
{
    private void Awake()
    {
        // 로컬 플레이어 설정
        GambleManager.SetLocalPlayer(this);
    }

    // 코인, 메달, 총, 로그 보드 등의 게임 오브젝트 및 이펙트에 대한 참조 변수 선언
    public GameObject coin, coinSpawner;
    public GameObject medalSpawner;
    public GameObject gun, gunSpawner;
    public GameObject chest;
    public GameObject logBoardSpawner;
    public GameObject playerZone;
    public GameObject disappearEffect, choseEffect;
    public GameObject[] medalObjects = new GameObject[3];
    static GameObject[] medals = new GameObject[3];
    public GameObject logBoard;
    private LogBoardContents logBoardContents;

    // 로그 보드 스폰 여부를 나타내는 변수
    private bool isBoardSpawned = false;

    // 메달을 스폰하는 메서드
    public void SpawnMedals()
    {
        // 메달 스폰 이펙트 재생
        PlayMedalSpawnEffect();

        Transform tr = medalSpawner.transform;

        // 메달 스폰 위치 계산 및 인스턴스화
        for (int i = 0; i < medals.Length; i++)
        {
            Vector3 pos = tr.position + transform.right * 0.2f * i;
            medals[i] = PhotonNetwork.Instantiate(medalObjects[i].name, pos, tr.rotation);
            medals[i].transform.parent = tr;
        }
    }

    // 메달을 제거하는 메서드
    public void DestroyMedals()
    {
        for (int i = 0; i < medals.Length; i++)
        {
            if (medals[i] == null) continue;
            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    // 메달을 제거하면서 이펙트를 재생하는 메서드
    public void DestroyMedalsWithEffect(Choice choice)
    {
        Transform tr = null;
        for (int i = 0; i < medals.Length; i++)
        {
            if (medals[i] == null) continue;

            tr = medals[i].transform;

            // 선택한 메달에 따라 다른 이펙트 재생
            if (choice == medals[i].GetComponent<Medal>().choice)
                PhotonNetwork.Instantiate(choseEffect.name, tr.position, tr.rotation);
            else
                PhotonNetwork.Instantiate(disappearEffect.name, tr.position, tr.rotation);

            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    // 메달을 재스폰하는 메서드
    public void ReSpawnMedals()
    {
        // 플레이어 존 이펙트 재생
        PlayPlayerZoneEffect();
        DestroyMedals();
        PlayMedalSpawnEffect();
        SpawnMedals();
    }

    // 코인을 추가하는 메서드
    public void AddCoins(int count)
    {
        Transform tr = coinSpawner.transform;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = tr.position + tr.up * 0.1f * i;
            GameObject obj = PhotonNetwork.Instantiate(coin.name, pos, Quaternion.identity);
            obj.transform.parent = coinSpawner.transform;
        }
    }

    // 모든 코인을 제거하고 상자의 코인 개수를 증가시키는 메서드
    public void RemoveCoins()
    {
        Transform[] coinList = coinSpawner.GetComponentsInChildren<Transform>();
        foreach (var c in coinList)
        {
            if (c.gameObject == coinSpawner) continue;
            PhotonNetwork.Destroy(c.gameObject);
            GambleManager.chestCoins++;
        }
    }

    // 특정 높이 이하의 코인을 제거하고 상자의 코인 개수를 증가시키는 메서드
    // 현재는 y값으로만 처리하도록 되어있음
    public void RemoveCoins(int yConstraint)
    {
        Transform[] coinList = coinSpawner.GetComponentsInChildren<Transform>();
        foreach (var c in coinList)
        {
            if (c.gameObject == coinSpawner) continue;
            if (c.position.y < yConstraint) PhotonNetwork.Destroy(c.gameObject);
            GambleManager.chestCoins++;
        }
    }

    // 총을 스폰하는 메서드
    public void SpawnGun()
    {
        Transform tr = gunSpawner.transform;
        PhotonNetwork.Instantiate(gun.name, tr.position, tr.rotation);
    }

    // 총을 제거하는 메서드
    public void DestroyGun()
    {
        GameObject gunInstance = gunSpawner.GetComponentInChildren<GunManager>().gameObject;
        PhotonNetwork.Destroy(gunInstance);
    }

    // 플레이어 존 이펙트를 재생하는 메서드
    public void PlayPlayerZoneEffect()
    {
        var particles = playerZone.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in particles) p.Play();
    }

    // 메달 스폰 이펙트를 재생하는 메서드
    public void PlayMedalSpawnEffect()
    {
        var particles = medalSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = medalSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }

    // 로그 보드를 스폰하는 메서드
    public void SpawnLogBoard()
    {
        // 이미 스폰되었다면 더 이상 스폰하지 않음
        if (isBoardSpawned) return;

        // 로그 보드 스폰 이펙트 재생
        PlayLogBoardSpawnEffect();

        // 로그 보드를 인스턴스화
        logBoard = PhotonNetwork.Instantiate(logBoard.name, logBoardSpawner.transform.position, Quaternion.identity);
        logBoardContents = logBoard.transform.GetComponentInChildren<LogBoardContents>();
        isBoardSpawned = true;
    }

    // 로그 보드에 메시지를 출력하는 메서드
    public void LogOnBoard(string message)
    {
        logBoardContents.LogMessage(message);
    }

    // 로그 보드 스폰 이펙트를 재생하는 메서드
    public void PlayLogBoardSpawnEffect()
    {
        var particles = logBoardSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = logBoardSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }
}
