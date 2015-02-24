using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
public class TextExtended : MonoBehaviour 
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Transform trans;
    public Transform Trans
    {
        get { return trans; }
    }

    public void SetText(string text)
    {
        #if FULL_DEBUG
        if(String.IsNullOrEmpty(text))
        {
            Debug.LogError("null or empty text");
            this.text.text = "";
        }
        #endif
        this.text.text = text;
    }

}
