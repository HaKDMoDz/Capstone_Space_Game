using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SystemLog : MonoBehaviour {

    private static ArrayList messageLog;
    private Text text;

	void Start () 
    {
        text = GameObject.Find("Log").GetComponent<Text>();
        messageLog = new ArrayList();
	}
	
	void Update () 
    {
        for (int i = messageLog.Count - 1; i > 0; i--)
        {
            text.text += messageLog[i]; //print each message
            text.text += ".\n"; // add a period and newline
            messageLog.Remove(messageLog[i]); //remove it from the messageLog
        }
        
	}

    public static void addMessage(string message)
    {
        messageLog.Add(message);       
    }

}
