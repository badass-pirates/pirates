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
    static GameObject[] medals = new GameObject[3];
    const int share = 0, challenge = 1, attack = 2;
    static string[] medalNames = {"MedalShare", "MedalChallenge", "MedalAttack"};

    public void SpawnMedals()
    {
        PlayMedalEffect();
        Transform tr = medalSpawner.transform;
        for(int i = 0; i < medals.Length; i++)
        {   
            Vector3 pos = tr.position + transform.right * 0.1f * i;
            medals[i] = PhotonNetwork.Instantiate(medalNames[i], pos, tr.rotation);
            medals[i].transform.parent = tr;
        }
    }

    public void DestroyMedals()
    {
        for(int i = 0; i < medals.Length; i++)
        {
            PhotonNetwork.Destroy(medals[i]);
            medals[i] = null;
        }
    }

    public void DestroyMedalsWithEffect(Choice choice)
    {
        for(int i = 0; i < medals.Length; i++)
        {
            medals[i].GetComponent<Medal>().Destroy(choice);
            medals[i] = null;
        }
    }

    public void ReSpawnMedals()
    {
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
            GameObject obj = PhotonNetwork.Instantiate("Coin", tr.position + new Vector3(0, i, 0), Quaternion.identity);
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
