/*
  TextExtended.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 23/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
    private RectTransform rectTrans;
    public RectTransform RectTrans
    {
        get { return rectTrans; }
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
