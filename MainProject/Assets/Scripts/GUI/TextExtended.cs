/*
  TextExtended.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 23/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion Usings
public class TextExtended : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private RectTransform rectTrans;
    public RectTransform RectTrans
    {
        get { return rectTrans; }
    }

    private delegate void TextPointerEnterEvent();
    private event TextPointerEnterEvent OnTextPointerEnter = new TextPointerEnterEvent(() => { });
    private delegate void TextPointerExitEvent();
    private event TextPointerExitEvent OnTextPointerExit = new TextPointerExitEvent(() => { });

    public void ShowText(bool show)
    {
        text.gameObject.SetActive(show);
    }

    public void SetText(string text)
    {
        #if FULL_DEBUG
        if(String.IsNullOrEmpty(text))
        {
            Debug.LogError("null or empty text");
            this.text.text = "";
        }
        if(!this.text.IsActive())
        {
            Debug.LogError("Text component is not active");
        }
        #endif
        this.text.text = text;
    }
    public void SetTextColour(Color textColour)
    {
        text.color = textColour;
    }
    public void AddOnPointerEnterListener(UnityAction action)
    {
        OnTextPointerEnter += () => action();
    }
    public void AddOnPointerExitListener(UnityAction action)
    {
        OnTextPointerExit += () => action();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("pointer enter");
        OnTextPointerEnter();   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("pointer exit");
        OnTextPointerExit();
    }
}
