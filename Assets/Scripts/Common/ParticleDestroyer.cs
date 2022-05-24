using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleDestroyer : MonoBehaviour
{
    private void OnDestroy() {
        PhotonNetwork.Destroy(gameObject);
    }
}
