/*
  InputFieldExtended.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 21/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class InputFieldExtended : MonoBehaviour 
{

    [SerializeField]
    private InputField inputField;
    public InputField InputField
    {
        get { return inputField; }
    }

    public void AddOnSubmitListener(UnityAction<string> action)
    {
        //inputField.onEndEdit.AddListener((value) => action(value));
        inputField.onEndEdit.AddListener((value) => ValidateSubmitAction(value, action));
    }

    private void ValidateSubmitAction(string value, UnityAction<string> action)
    {
        if (Input.GetButtonDown("Submit"))
        {
            action(value);
        }
    }
}
