using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GambleManager : MonoBehaviour
{
    State state;

    const int POT_WEIGHT = 1;
    const int MAX_DECIDE_TIME = 60;
    const int MAX_ATTACK_TIME = 60;

    int leftTime;
    public TextMeshProUGUI timeText;

    int round, act;
    int pot;
    List<PlayerInfo> playerList = new List<PlayerInfo>();
    PlayerInfo challengeWinner = null, attackWinner = null;

    int GetPotGold(int round)
    {
        int weight = (int)Mathf.Pow(POT_WEIGHT, round-1);
        int min = weight*10;
        return Random.Range(min, min*(round+1));
    }

    void Decide()
    {
        if(leftTime > 0)
        {
            foreach(var p in playerList)
            {
                if(p.choice == Choice.ready)
                { 
                    leftTime--; 
                    timeText.text = leftTime.ToString(); 
                    return; 
                } 
            }
        }
        else
        {
            foreach(var p in playerList)
            {
                if(p.choice == Choice.ready) p.choice = Choice.share;
            }
        }
        state = State.check;
    }

    void Check()
    {
        int attackCount = 0;
        foreach(var p in playerList)
        {
            switch(p.choice)
            {
                case Choice.share : p.isSuccess  = true; break;

                case Choice.challenge : 
                    if(challengeWinner.challengeAmount < p.challengeAmount)
                        challengeWinner = p;
                    break;

                case Choice.attack :
                    if(attackCount == 0) attackWinner = p;
                    p.attackChance--;
                    attackCount++;
                    break;

                default : p.choice = Choice.share; p.isSuccess = true; break;
            }
            //챌린지, 어택(1명인 경우만) 승자에게 true 부여
            challengeWinner.isSuccess = true;
            if(attackCount == 1) 
            {
                attackWinner.isSuccess = true;
                leftTime = MAX_ATTACK_TIME;
                state = State.attack;
            }
            state = State.apply;
        }
    }

    void Attack()
    {
        if(!attackWinner.isShoot || leftTime > 0)
            leftTime--;
        else state = State.apply;
        //Kill 했을 때 네트워크에 해당 금액 제공
    }

    void Apply()
    {
        //네트워크에 각자 T 결과에 따라 처리하도록 전송
    }

    void Start()
    {
        
    }

    void Update()
    {

    }
}