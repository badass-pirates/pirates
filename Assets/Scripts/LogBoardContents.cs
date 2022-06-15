using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBoardContents : MonoBehaviour
{
    public GameObject logMessage;

    private void Start()
    {
        CreateLogMessage("round start!");
    }

    private void CreateLogMessage(string message)
    {
        GameObject q = Instantiate(logMessage);
        q.GetComponent<LogMessageInfo>().SetTextUI(message);
        q.transform.SetParent(transform, false);
    }
}
