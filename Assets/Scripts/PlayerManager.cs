using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //medal 등이 NetworkPlayer 에서 생성됨 
    GameObject medalShare, medalChallenge, medalAttack;
    public GameObject coinSpawner;
    GameObject chest;
    int coinsInChest;
    CoinPile coinPile;

    int challengeAmount;

    public void SetChest(GameObject _chest)
    {
        chest = _chest;
    }

    public void SetMedals(GameObject share, GameObject challenge, GameObject attack)
    {
        medalShare = share;
        medalChallenge = challenge;
        medalAttack = attack;
    }

    public int GetCoinsCount()
    {
        return coinPile.Count() + coinsInChest;
    }

    //이 함수가 던져졌을 때 체크하는 함수랑 똑같음
    public GameObject GetMouseTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.transform.gameObject;

        return null;
    }

    //VR로 던져졌을 때, 던져진 오브젝트를 obj에 넣으면 됨
    public Choice GetPlayerChoice(GameObject obj)
    {
        if (obj == null) return Choice.none;
        else if (obj == medalShare) return Choice.share;
        else if (obj == medalChallenge) return Choice.challenge;
        else if (obj == medalAttack) return Choice.attack;
        else return Choice.none;
    }

    public void Decide(GameObject obj)
    {
        if (GambleManager.GetState() == State.decide)
        {
            Choice choice = GetPlayerChoice(obj);
            Debug.Log(choice);

            if (choice == Choice.share)
                GambleManager.SetPlayerChoice(choice);

            else if (choice == Choice.challenge)
            {
                GambleManager.SetPlayerChoice(choice);
                GambleManager.SetPlayerChallengeAmount(challengeAmount);//우변 challengeAmount 를 UI에서 리턴받은 값으로 넣어주면 됨
            }

            else if (choice == Choice.attack)
            {
                if (GambleManager.GetPlayerInfo().attackChance < 1)
                    return;
            }
            else return;
        }
    }

    public void Decide(GameObject obj, int amount)
    {
        challengeAmount = amount;
        Decide(obj);
    }

    public void AddCoinsFromDiff(int current)
    {
        int diff = current - GetCoinsCount();
        if (diff == 0) return;

        coinPile.AddCoins(current - GetCoinsCount());
    }

    public void GetCoinWithMouse()
    {
        GameObject obj = GetMouseTarget();
        if (obj.tag == "coin")
        {
            Destroy(obj);
            coinsInChest++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        coinPile = new CoinPile(coinSpawner);
        coinsInChest = 0;
        challengeAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //던져졌을 때
        if (Input.GetMouseButtonDown(0))
        {
            Decide(GetMouseTarget());
        }
        AddCoinsFromDiff(GambleManager.GetPlayerInfo().coins);
        GetCoinWithMouse();
        //challengeAmount = Random.Range(1, GambleManager.maxPotCoins);
    }
}
