using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MicrophoneListener : MonoBehaviour
{
    public float sensitivity = 100;
    public float loudness = 0;
    AudioSource _audio;

    public GameObject component;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
        _audio.clip = Microphone.Start(null, true, 10, 44100);
        _audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        _audio.Play();
    }

    void Update()
    {
        loudness = GetAveragedVolume() * sensitivity;
        Debug.Log(loudness);
        ChangeComponentScale();
    }

    void ChangeComponentScale()
    {
        component.transform.localScale = new Vector3(loudness, loudness, loudness);
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }
}
