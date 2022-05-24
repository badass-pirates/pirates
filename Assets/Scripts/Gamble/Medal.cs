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
        collide = other.gameObject;
        if(other.gameObject == playerZone)
            isCorrectPos = true;
        else if (other.gameObject == choiceZone)
        {
            isCorrectPos = true;
            passTime = 0f;
            if(choice == Choice.challenge)
            {
                ChallengeAmount cAmount = GetComponent<ChallengeAmount>();
                GambleManager.DecidePlayerChallenge((int)cAmount.amount);
                return;
            }
            GambleManager.DecideChoice(choice);
        }

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == playerZone || other.gameObject == choiceZone)
            isCorrectPos = false;
    }

    void Awake()
    {
        playerZone = GameObject.Find("PlayerZone").gameObject;
        choiceZone = GameObject.Find("ChoiceZone").gameObject;
        if(!gameObject.GetComponent<PhotonView>().IsMine) Destroy(this);
    }

    void Update()
    {
        if (!isCorrectPos)
            passTime += Time.deltaTime;
        if (passTime >= timeConstraint)
        {
            GambleManager.localPlayer.ReSpawnMedals();
            passTime = 0f;
        }
    }
}
