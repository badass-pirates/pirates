using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject medalShare, medalChallenge, medalAttack;
    CoinPile coins;
    public int playerIndex{get; set;}

    void ActiveMedal()
    {
        medalShare.SetActive(true);
        medalChallenge.SetActive(true);
        medalAttack.SetActive(true);
    }

    public GameObject GetMouseTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.transform.gameObject);
            return hit.transform.gameObject;
        }
        return null;
    }
    
    public Choice GetPlayerChoice()
    {
        GameObject obj = GetMouseTarget();

        if(obj == medalShare) return Choice.share;
        else if(obj == medalChallenge) return Choice.challenge;
        else if(obj == medalAttack) return Choice.attack;
        else return Choice.none;
    }

    public void Decide()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Choice c = GetPlayerChoice();
            if(c == Choice.none) return;
            else GambleManager.GetPlayerInfo().choice = c;
        }
    }

    public void Apply(int num)
    {
        Debug.Log(coins.transform.position);
        coins.AddCoins(num);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        playerIndex = GambleManager.playerIndex;
    }

    // Update is called once per frame
    void Update()
    {
        switch(GambleManager.GetState())
        {
            case State.decide :Decide(); break;
            case State.check : break;
            case State.attack : break;
            case State.apply : break;
        }

    }
}
