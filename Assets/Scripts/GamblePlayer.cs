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
    static GameObject medalShare, medalChallenge, medalAttack;

    public void SpawnMedals()
    {
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
        Destroy(medalShare);
        Destroy(medalChallenge);
        Destroy(medalAttack);
        //null 해야할 거 같은데
    }

    public void ReSpawnMedals()
    {
        DestroyMedals();
        SpawnMedals();
    }

    public void AddCoins(int count)
    {
        Transform tr = coinSpawner.transform;
        for (int i = 0; i < count; i++)
        {
            //동전생성모양 설정 가능
            //GameObject obj = Instantiate(coin, tr.position + new Vector3(0, i, 0), Quaternion.identity);
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
            Destroy(c.gameObject);
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
            if(c.position.y < yConstraint) Destroy(c.gameObject);
            GambleManager.chestCoins++;
        }
    }
    public void RemoveCoinByIndex(int index)
    {
        Destroy(coinSpawner.transform.GetChild(index));
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
        if (obj.tag == "coin")
        {
            Destroy(obj);
            GambleManager.chestCoins++;
        }
    }
    public void GetPlayerChoiceByMouse()
    {
        if(Input.GetMouseButton(0))
        {
            GameObject obj = GetMouseTarget();
            if(obj == medalShare) GambleManager.SetPlayerChoice(Choice.share);
            else if(obj == medalChallenge) GambleManager.SetPlayerChoice(Choice.challenge);
            else if(obj == medalAttack) GambleManager.SetPlayerChoice(Choice.attack);
            else return;
        }
    }

    private void Update() {
        GetCoinWithMouse();
        GetPlayerChoiceByMouse();
    }
}
