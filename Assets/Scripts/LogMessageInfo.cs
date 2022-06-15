using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogMessageInfo : MonoBehaviour
{
    public TextMeshProUGUI messageUI;

    public void SetTextUI(string message)
    {
        messageUI.text = message;
    }
}


