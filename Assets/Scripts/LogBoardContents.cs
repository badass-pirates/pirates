using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBoardContents : MonoBehaviour
{
    public GameObject logMessage;

    private List<GameObject> messageList = new List<GameObject>();
    private Vector3 defaultScale;

    public void LogMessage(string message)
    {
        GameObject messageObject = Instantiate(logMessage);
        messageList.Add(messageObject);
        if (messageList.Count > 8)
        {
            defaultScale = messageObject.transform.lossyScale;
            transform.localScale += new Vector3(0, 0.1f, 0);
            foreach (GameObject msg in messageList)
            {
                msg.transform.localScale = defaultScale;
            }
        }
        messageObject.GetComponent<LogMessageInfo>().SetTextUI(message);
        messageObject.transform.SetParent(transform, false);
    }
}
