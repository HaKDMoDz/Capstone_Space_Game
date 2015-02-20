using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeaderText : MonoBehaviour 
{
    [SerializeField]
    private Text headerText;

    public void SetText(string text)
    {
        #if FULL_DEBUG
        if(String.IsNullOrEmpty(text))
        {
            Debug.LogError("Button text is null or empty");
            headerText.text = "";
            return;
        }
        #endif
        headerText.text = text;
    }

}
