using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : PhotonVoiceNetwork
{
    public static VoiceManager instance;
    void Awake()
    {
        if (instance != null) return;
        instance = this;
    }
}
