using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ParticleDestroyer : MonoBehaviour
{
    private void OnParticleTrigger()
    {
        if (this.GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
