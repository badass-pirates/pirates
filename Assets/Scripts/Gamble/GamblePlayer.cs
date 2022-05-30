using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GamblePlayer : MonoBehaviour
{
    private void Awake()
    {
        GambleManager.SetLocalPlayer(this);
    }

    public GameObject coin, chest, coinSpawner, medalSpawner;
    public GameObject playerZone;
    public GameObject disappearEffect, choseEffect;
    public GameObject[] medalObjects = new GameObject[3];
    static GameObject[] medals = new GameObject[3];

    public void SpawnMedals()
    {
        Debug.Log("Spawn Medals"+PhotonNetwork.localPlayer.ActorNumber);
        PlayMedalEffect();
        Transform tr = medalSpawner.transform;
        for(int i = 0; i < medals.Length; i++)
        {   
            Vector3 pos = tr.position + transform.right * 0.1f * i;
            medals[i] = PhotonNetwork.Instantiate(medalObjects[i].name, tr.position, tr.rotation);
            medals[i].transform.parent = tr;
        }
    }

    public void DestroyMedals()
    {
        for(int i = 0; i < medals.Length; i++)
        {   if(medals[i]!=null)
            {
                PhotonNetwork.Destroy(medals[i]);
                Debug("Destroy "+ medals[i].GetComponent<Medal>().choice);
            }
            else
                Debug("Destroy failed "+ medals[i].GetComponent<Medal>().choice);
        }
    }

    public void DestroyMedalsWithEffect(Choice choice)
    {
        Transform tr = null;
        for(int i = 0; i < medals.Length; i++)
        {
            tr = medals[i].transform;
            if(choice == medals[i].GetComponent<Medal>().choice) 
                PhotonNetwork.Instantiate(choseEffect.name, tr.position, tr.rotation);
            else
                PhotonNetwork.Instantiate(disappearEffect.name, tr.position, tr.rotation);
                
            Debug("Destroy "+ medals[i].GetComponent<Medal>().choice);
            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    public void ReSpawnMedals()
    {
        Debug.Log("Medal Respawn", PhotonNetwork.localPlayer.ActorNumber);
        PlayPlayerZoneEffect();
        DestroyMedals();
        PlayMedalEffect();
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
    //현재는 y값으로만 처리하도록 되어있음
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

    //그래픽 처리
    public void PlayPlayerZoneEffect()
    {
        var particles = playerZone.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in particles) p.Play();
    }

    public void PlayMedalEffect()
    {
        var particles = medalSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = medalSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }
}
