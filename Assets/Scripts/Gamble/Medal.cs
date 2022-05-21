using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public Choice choice;
    GameObject playerZone, choiceZone;

    bool isCorrectPos;
    const float timeConstraint = 3f;
    float passTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == playerZone)
            isCorrectPos = true;
        else if(other.gameObject == choiceZone)
        {
            passTime = 0f;
            GambleManager.SetPlayerChoice(choice);
            return;
        }

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == playerZone)
            isCorrectPos = false;
    }

    void Awake() 
    {
        playerZone = GameObject.Find("PlayerZone");
        choiceZone = GameObject.Find("ChoiceZone");
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
