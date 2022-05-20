using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCollidePlay : MonoBehaviour
{
    public AudioSource collideSound;

    private void OnCollisionEnter(Collision other) {
        collideSound.Play();
    }
}
