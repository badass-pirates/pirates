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
            pm.Decide(other.gameObject);
        }
    }
}
