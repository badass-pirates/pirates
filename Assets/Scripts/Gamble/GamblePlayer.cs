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
    static GameObject medalShare, medalChallenge, medalAttack;

    public void SpawnMedals()
    {
        ShowMedalEffect();
        Transform tr = medalSpawner.transform;
        medalShare = PhotonNetwork.Instantiate("MedalShare", tr.position, tr.rotation);
        medalShare.transform.parent = tr;
        medalChallenge = PhotonNetwork.Instantiate("medalChallenge", tr.position, tr.rotation);
        medalChallenge.transform.parent = tr;
        medalAttack = PhotonNetwork.Instantiate("MedalAttack", tr.position, tr.rotation);
        medalAttack.transform.parent = tr;
    }

    public void DestroyMedals()
    {
        PhotonNetwork.Destroy(medalShare);
        PhotonNetwork.Destroy(medalChallenge);
        PhotonNetwork.Destroy(medalAttack);
    }

    public void ReSpawnMedals()
    {
        ShowPlayerZone();
        DestroyMedals();
        ShowMedalEffect();
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
        foreach(var c in coinList)
        {
            if(c.gameObject == coinSpawner) continue;
            PhotonNetwork.Destroy(c.gameObject);
            GambleManager.chestCoins++;
        }
    }
    //현재는 y값으로만 처리하도록 되어있음
    public void RemoveCoins(int yConstraint)
    {
        Transform[] coinList = coinSpawner.GetComponentsInChildren<Transform>();
        foreach(var c in coinList)
        {
            if(c.gameObject == coinSpawner) continue;
            if(c.position.y < yConstraint) PhotonNetwork.Destroy(c.gameObject);
            GambleManager.chestCoins++;
        }
    }

    //그래픽 처리
    public void ShowPlayerZone()
    {
        var particles = playerZone.transform.GetComponentsInChildren<ParticleSystem>();
        foreach(var p in particles) p.Play();
    }

    public void ShowMedalEffect()
    {
        var particles = medalSpawner.transform.GetComponentInChildren<ParticleSystem>();
        var fx = medalSpawner.transform.GetComponentInChildren<AudioSource>();
        particles.Play();
        fx.Play();
    }

    //마우스 처리
    public GameObject GetMouseTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.transform.gameObject;

        return null;
    }
    public void GetCoinWithMouse()
    {
        GameObject obj = GetMouseTarget();
        if(obj == null) return;
        else if (obj.tag == "coin")
        {
            PhotonNetwork.Destroy(obj);
            GambleManager.chestCoins++;
        }
    }
    public void GetPlayerChoiceByMouse()
    {
        if(Input.GetMouseButton(0))
        {
            GameObject obj = GetMouseTarget();
            if(obj == medalShare) GambleManager.DecideChoice(Choice.share);
            else if(obj == medalChallenge) GambleManager.DecideChoice(Choice.challenge);
            else if(obj == medalAttack) GambleManager.DecideChoice(Choice.attack);
            else return;
        }
    }

    private void Update() {
        GetCoinWithMouse();
        GetPlayerChoiceByMouse();
    }
}
