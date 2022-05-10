using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    GameObject chest;
    void Start()
    {
        chest = PhotonNetwork.Instantiate("Chest", transform.position, transform.rotation);
        chest.transform.parent = gameObject.transform;
        GetComponentInParent<PlayerManager>().SetChest(chest);
    }
}
