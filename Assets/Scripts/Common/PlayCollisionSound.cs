using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCollisionSound : MonoBehaviour
{
    public AudioSource collideSound;

    private void OnCollisionEnter(Collision other) {
        collideSound.Play();
    }
}
