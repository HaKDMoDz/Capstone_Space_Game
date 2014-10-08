using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SystemLog : MonoBehaviour 
{
    private static List<string> messageLog;
    private static Text text;

	void Awake () 
    {
        text = GameObject.Find("Log").GetComponent<Text>();

        messageLog = new List<string>();
        StartCoroutine(removeMessage());
	}


    static void refreshTextBox()
    {
        text.text = "";

        foreach (string message in messageLog)
        {
            string sysDateTime = System.DateTime.Now.ToShortTimeString();
            text.text += "[" + sysDateTime + "]:" + message; //print each message
            text.text += ".\n"; // add a period and newline

        }
    }

    public static void addMessage(string message)
    {
        messageLog.Add(message);
        refreshTextBox();
    }


    IEnumerator removeMessage()
    {
        yield return new WaitForSeconds(5.0f);
        
        if (messageLog.Count > 0)
        {
            messageLog.Remove(messageLog[0]); //remove it from the messageLog
        }

        StartCoroutine(removeMessage());
   }  
   
    public void disableMe()
    {
        transform.parent.parent.parent.GetComponent<Canvas>().enabled = false;

    }

    public void enableMe()
    {
        transform.parent.parent.parent.GetComponent<Canvas>().enabled = true;
    }

    public void toggleEnable()
    {
        Canvas canvas = transform.parent.parent.parent.GetComponent<Canvas>();
        canvas.enabled = !canvas.enabled;
    }
}
