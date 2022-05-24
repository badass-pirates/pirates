using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Medal : MonoBehaviour
{
    public Choice choice;
    public GameObject localPlayer;
    public GameObject playerZone, choiceZone;
    public GameObject collide;

    bool isCorrectPos;
    const float timeConstraint = 2f;
    float passTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if(!gameObject.GetComponent<PhotonView>().IsMine)
        { 
            enabled = false;
            return;
        }
        collide = other.gameObject;
        Debug.Log(gameObject.GetComponent<PhotonView>().IsMine+","+isCorrectPos + ", "+ choice+","+collide);
        if(other.gameObject == playerZone)
            isCorrectPos = true;
        else if(other.gameObject == choiceZone)
        {
            Time.timeScale = 0;
            passTime = 0f;
            GambleManager.SetPlayerChoice(choice);
            if(choice == Choice.challenge)
            {
                ChallengeAmount cAmount = GetComponent<ChallengeAmount>();
                GambleManager.SetPlayerChallengeAmount(cAmount.amount);
            }
        }

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == playerZone)
            isCorrectPos = false;
    }

    void Awake() 
    {
        playerZone = GameObject.Find("PlayerZone").gameObject;
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
