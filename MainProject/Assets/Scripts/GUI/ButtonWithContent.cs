using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonWithContent : MonoBehaviour 
{
    [SerializeField]
    private Button button;
    public Button Button
    {
        get { return button; }
    }
    [SerializeField]
    private Text buttonText;
    	
    public void SetText(string text)
    {
        #if FULL_DEBUG
        if(String.IsNullOrEmpty(text))
        {
            Debug.LogError("Button text is null or empty");
            buttonText.text = "";
            return;
        }
        #endif
        buttonText.text = text;
    }

    public void AddOnClickListener(UnityAction action)
    {
        button.onClick.AddListener(action);
    }
    public void RemoveOnClickListeners()
    {
        button.onClick.RemoveAllListeners();
    }
}
