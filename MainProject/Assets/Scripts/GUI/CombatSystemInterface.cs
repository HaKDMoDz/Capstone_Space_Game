#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion Usings

#region AdditionalStructs
[Serializable]
public struct CombatGUIFields
{
    public ButtonWithContent buttonPrefab;
    public RectTransform turnOrderButtonParent;
}
#endregion AdditionalStructs
public class CombatSystemInterface : Singleton<CombatSystemInterface>
{
    #region Fields
    #region EditorExposed
    [SerializeField]
    private CombatGUIFields guiFields;
    #endregion EditorExposed

    //private List<RectTransform> shipButtonRectList;
    private Dictionary<TurnBasedUnit, ButtonWithContent> unit_buttonRect_table;
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    #region GUISetup
    public void AddShipButton(TurnBasedUnit unit)
    {
        ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
        buttonClone.buttonText.text = unit.shipBPMetaData.blueprintName;
        //RectTransform buttonRect = buttonClone.transform as RectTransform;
        buttonClone.transform.SetParent(guiFields.turnOrderButtonParent, false);
        unit_buttonRect_table.Add(unit, buttonClone);
    }
    public void UpdateTurnOrderPanel(List<TurnBasedUnit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            ButtonWithContent button = unit_buttonRect_table[units[i]];
            button.buttonText.text = units[i].shipBPMetaData.blueprintName + " : " + units[i].TimeLeftToTurn;
            button.transform.SetSiblingIndex(i);
        }
    }
    #endregion GUISetup

    #endregion PublicMethods

    #region PrivateMethods

    #region UnityCallbacks

    private void Awake()
    {
        unit_buttonRect_table = new Dictionary<TurnBasedUnit, ButtonWithContent>();
    }

    #endregion UnityCallbacks

    #endregion PrivateMethods

    #endregion Methods
}
