/*
  MainMenuController.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 10/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
#endregion Usings

public class MainMenuController : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private GUIFields guiFields;
    #endregion Fields

    #region Methods
    #region GUI_Callbacks
    public void Continue()
    {
        Debug.Log("Continue");
        GameController.Instance.LoadLatestSave();
    }
    public void NewGame()
    {
        Debug.Log("new game");
        GameController.Instance.StartNewGame();
    }
    public void LoadGame()
    {
        Debug.Log("Load Game");
    }
    public void Options()
    {
        Debug.Log("options");
    }
    public void Credits()
    {
        Debug.Log("Credits");
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    #endregion GUI_Callbacks
    #region Private
    private void Start()
    {
        SetupMainMenuButtons();
        MainMenuOptions.Instance.Init();
        AudioManager.Instance.SetMainTrack(Sound.SciFiTheme);
    }
    private void SetupMainMenuButtons()
    {
        if(GameController.Instance.AnySavesExist())
        {
            guiFields.continueButton.SetActive(true);
        }
    }
    #endregion Private
    #endregion Methods
}
#region AdditionalStructs
[Serializable]
public struct GUIFields
{
    public GameObject continueButton;
}
#endregion AdditionalStructs