using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌 개체 : " + other.gameObject.name);
        if (other.gameObject.tag == "medal")
        {
            PlayerManager pm = GameObject.FindGameObjectWithTag("local_player").GetComponent<PlayerManager>();
            if (other.gameObject.name == "MedalChallenge")
            {
                int amount = other.gameObject.GetComponent<ChallengeAmount>().Amount;
                pm.Decide(other.gameObject, amount);
            }
            else pm.Decide(other.gameObject);
        }
    }
}
