/*
  Button_TextAndImage.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 28/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class Button_TextAndImage : MonoBehaviour 
{
    [SerializeField]
    private Button button;
    public Button Button
    {
        get { return button; }
    }

    [SerializeField]
    private Image image;
    [SerializeField]
    private Text buttonText;

    public void SetSprite(Sprite sprite)
    {
#if FULL_DEBUG
      if(sprite == null)
      {
          Debug.LogError("Sprite is null");
          return;
      }
#endif
      this.image.sprite = sprite;
    }
    
    public void SetText(string text)
    {
#if FULL_DEBUG
        if (String.IsNullOrEmpty(text))
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
