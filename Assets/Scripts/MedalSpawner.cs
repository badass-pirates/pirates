using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MedalSpawner : MonoBehaviour
{
    GameObject medalShare, medalChallenge, medalAttack;
    
    public void SpawnMedals()
    {
        medalShare = PhotonNetwork.Instantiate("MedalShare", transform.position, transform.rotation);
        medalShare.transform.parent = gameObject.transform;
        medalChallenge = PhotonNetwork.Instantiate("MedalChallenge", transform.position, transform.rotation);
        medalChallenge.transform.parent = gameObject.transform;
        medalAttack = PhotonNetwork.Instantiate("MedalAttack", transform.position, transform.rotation);
        medalAttack.transform.parent = gameObject.transform;

        GetComponentInParent<PlayerManager>().SetMedals(medalShare, medalChallenge, medalAttack);
    }

    public void DestroyMedals()
    {
        Destroy(medalShare);
        Destroy(medalChallenge);
        Destroy(medalAttack);

        GetComponentInParent<PlayerManager>().SetMedals(null, null, null);
    }
}
