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
    public float scale = 1f;
    public float minumScale = 1f;
    public float maximumScale = 3f;
    public float pivotLoundness;

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
        ChangeComponentScale();
    }

    void ChangeComponentScale()
    {
        scale += (loudness > pivotLoundness) ? Time.deltaTime : -Time.deltaTime;
        if (scale < minumScale)
        {
            scale = minumScale;
        }
        if (scale > maximumScale)
        {
            scale = maximumScale;
        }
        component.transform.localScale = new Vector3(scale, scale, scale);
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
