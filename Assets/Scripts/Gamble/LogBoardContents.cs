using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBoardContents : MonoBehaviour
{
    public GameObject logMessage;

    public void LogMessage(string message)
    {
        GameObject messageObject = Instantiate(logMessage);
        messageObject.GetComponent<LogMessageInfo>().SetTextUI(message);
        messageObject.transform.SetParent(transform, false);
    }
}
