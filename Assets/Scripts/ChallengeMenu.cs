using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeMenu : MonoBehaviour
{
    public GameObject chellengeUI;
    public bool activeChellengeUI = true;

    void Start()
    {
        DisplayChellengeUI();
    }

    public void DisplayChellengeUI()
    {
        if (activeChellengeUI)
        {
            chellengeUI.SetActive(false);
            activeChellengeUI = false;
        }
        else if (!activeChellengeUI)
        {
            chellengeUI.SetActive(true);
            activeChellengeUI = true;
        }
    }
}
