using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SystemLog : MonoBehaviour {

    private static List<string> messageLog;
    private Text text;

	void Awake () 
    {
        text = GameObject.Find("Log").GetComponent<Text>();

        messageLog = new List<string>();
        StartCoroutine(removeMessage());
	}

    void Update()
    {
        text.text = "";

        foreach (string message in messageLog)
        {
            text.text += message; //print each message
            text.text += ".\n"; // add a period and newline

        }
    }

    public static void addMessage(string message)
    {
        messageLog.Add(message);
    }

    IEnumerator removeMessage()
    {
        yield return new WaitForSeconds(2.0f);
        if (messageLog.Count > 0)
        {
            messageLog.Remove(messageLog[0]); //remove it from the messageLog
            StartCoroutine(removeMessage());
        }
   }     
}
