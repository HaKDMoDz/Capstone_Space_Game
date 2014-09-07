using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Info : MonoBehaviour
{
    private static List<string> infoMessage;
    private static Text text;
    private static Canvas myCanvas;

    void Awake()
    {
        text = GameObject.Find("Info").GetComponent<Text>();
        myCanvas = transform.parent.parent.parent.GetComponent<Canvas>();
        infoMessage = new List<string>();
        DisableMe();
    }

    public static void SetInfoToWindow()
    {
        text.text = "";

        foreach (string infoBit in infoMessage)
        {
            text.text += infoBit; //print each message
            text.text += ".\n"; // add a period and newline
        }
    }

    public static void ResetInfo()
    {
        infoMessage =  new List<string>();
    }

    public static void addMessage(string message)
    {
        infoMessage.Add(message);
    }

    public void disableMe()
    {
        myCanvas.enabled = false;

    }

    public static void DisableMe()
    {
        myCanvas.enabled = false;

    }

    public void enableMe()
    {
        myCanvas.enabled = true;
    }

    public static void EnableMe()
    {
        myCanvas.enabled = true;
    }

    void Update() { }
}
