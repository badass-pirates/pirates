using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class GamblePlayer : MonoBehaviour
{
    private void Awake()
    {
        GambleManager.SetLocalPlayer(this);
    }

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

    private bool isBoradSpawned = false;

    public void SpawnMedals()
    {
        PlayMedalSpawnEffect();
        Transform tr = medalSpawner.transform;
        for (int i = 0; i < medals.Length; i++)
        {
            Vector3 pos = tr.position + transform.right * 0.2f * i;
            medals[i] = PhotonNetwork.Instantiate(medalObjects[i].name, pos, tr.rotation);
            medals[i].transform.parent = tr;
        }
    }

    public void DestroyMedals()
    {
        for (int i = 0; i < medals.Length; i++)
        {
            if (medals[i] == null) continue;
            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    public void DestroyMedalsWithEffect(Choice choice)
    {
        Transform tr = null;
        for (int i = 0; i < medals.Length; i++)
        {
            if (medals[i] == null) continue;

            tr = medals[i].transform;
            if (choice == medals[i].GetComponent<Medal>().choice)
                PhotonNetwork.Instantiate(choseEffect.name, tr.position, tr.rotation);
            else
                PhotonNetwork.Instantiate(disappearEffect.name, tr.position, tr.rotation);

            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    public void ReSpawnMedals()
    {
        PlayPlayerZoneEffect();
        DestroyMedals();
        PlayMedalSpawnEffect();
        SpawnMedals();
    }

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

    public void SpawnGun()
    {
        Transform tr = gunSpawner.transform;
        PhotonNetwork.Instantiate(gun.name, tr.position, tr.rotation);
    }

    public void DestroyGun()
    {
        GameObject gunInstance = gunSpawner.GetComponentInChildren<GunManager>().gameObject;
        PhotonNetwork.Destroy(gunInstance);
    }

    //그래픽 처리
    public void PlayPlayerZoneEffect()
    {
        var particles = playerZone.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in particles) p.Play();
    }

    public void PlayMedalSpawnEffect()
    {
        var particles = medalSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = medalSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }

    public void SpawnLogBoard()
    {
        if (isBoradSpawned) return;
        PlayLogBoardSpawnEffect();
        logBoard = PhotonNetwork.Instantiate(logBoard.name, logBoardSpawner.transform.position, Quaternion.identity);
        logBoardContents = logBoard.transform.GetComponentInChildren<LogBoardContents>();
        isBoradSpawned = true;
    }

    public void LogOnBoard(string message)
    {
        logBoardContents.LogMessage(message);
    }

    public void PlayLogBoardSpawnEffect()
    {
        var particles = logBoardSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = logBoardSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }
}
