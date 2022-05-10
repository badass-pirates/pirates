using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MedalSpawner : MonoBehaviour
{
    GameObject medalShare, medalChallenge, medalAttack;
    
    public void SpawnMedal()
    {
        medalShare = PhotonNetwork.Instantiate("MedalShare", transform.position, transform.rotation);
        medalChallenge = PhotonNetwork.Instantiate("MedalChallenge", transform.position, transform.rotation);
        medalAttack = PhotonNetwork.Instantiate("MedalAttack", transform.position, transform.rotation);
        GetComponent<PlayerManager>().SetMedals(medalShare, medalChallenge, medalAttack);
    }

    public void DestroyMedal()
    {
        Destroy(medalShare);
        Destroy(medalChallenge);
        Destroy(medalAttack);
        GetComponent<PlayerManager>().SetMedals(null, null, null);
    }
}
