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
    #region EditorExposed
    [SerializeField]
    private GUIFields guiFields;
    #endregion EditorExposed
    #endregion Fields

    #region Methods
    #region Private
    private void Start()
    {
        SetupMainMenuButtons();
    }
    #region GUI
    private void SetupMainMenuButtons()
    {
        //Button newGame = Instantiate(guiFields.mainMenuButtonPrefab) as Button;
        //newGame.transform.SetParent(guiFields.buttonLayout.transform);
        //Text newGameText = newGame.GetComponentInChildren<Text>();
        //if(!newGameText)
        //{
        //    Debug.LogError("text not found");
        //}
        //newGameText.text  = "New Game";
        //Button options = Instantiate(guiFields.mainMenuButtonPrefab) as Button;
        //options.transform.SetParent(guiFields.buttonLayout.transform);
    }
    #endregion GUI
    #endregion Private
    #endregion Methods
}
#region AdditionalStructs
[Serializable]
public struct GUIFields
{
    public GameObject continueButton;
    //public LayoutGroup buttonLayout;
    //public Button mainMenuButtonPrefab;
}
#endregion AdditionalStructs