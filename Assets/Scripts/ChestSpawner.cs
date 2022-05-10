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
    }

    void Update()
    {
        if (chest != null)
        {
            GetComponentInParent<PlayerManager>().SetChest(chest);
        }
    }
}
