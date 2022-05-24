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
    static List<GameObject> medals = new List<GameObject>();

    public void SpawnMedals()
    {
        PlayMedalEffect();
        Transform tr = medalSpawner.transform;
        medals.Add(PhotonNetwork.Instantiate("MedalShare", tr.position + transform.right * 0, tr.rotation));
        medals.Add(PhotonNetwork.Instantiate("MedalChallenge", tr.position + transform.right * 0.1f, tr.rotation));
        medals.Add(PhotonNetwork.Instantiate("MedalAttack", tr.position + transform.right * 0.2f, tr.rotation));
        medals.ForEach(medal => medal.transform.parent = tr);
    }

    public void DestroyMedals()
    {
        medals.ForEach(medal => PhotonNetwork.Destroy(medal));
        medals = new List<GameObject>();
        Debug.Log(medals.Count);
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
