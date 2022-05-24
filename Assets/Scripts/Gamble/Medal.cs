using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public Choice choice;
    public GameObject localPlayer;
    GameObject playerZone, choiceZone;

    bool isCorrectPos;
    const float timeConstraint = 2f;
    float passTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerZone)
            isCorrectPos = true;
        else if(other.gameObject == choiceZone)
        {
            passTime = 0f;
            GambleManager.SetPlayerChoice(choice);
            if(choice == Choice.challenge)
            {
                ChallengeAmount cAmount = GetComponent<ChallengeAmount>();
                GambleManager.SetPlayerChallengeAmount(cAmount.amount);
            }
            return;
        }

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == playerZone)
            isCorrectPos = false;
    }

    void Awake() 
    {
        playerZone = localPlayer.transform.Find("PlayerZone").gameObject;
        choiceZone = GameObject.Find("ChoiceZone").gameObject;
    }

    void Update()
    {
        if(!isCorrectPos)
            passTime += Time.deltaTime;
        if(passTime >= timeConstraint)
        {
            GambleManager.localPlayer.ReSpawnMedals();
            passTime = 0f;
        }
    }
}
