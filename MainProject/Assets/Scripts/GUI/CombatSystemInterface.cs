﻿/*
  CombatSystemInterface.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 16/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
#endregion Usings

#region AdditionalStructs
[Serializable]
public struct CombatGUIFields
{
    //prefabs
    public ButtonWithContent buttonPrefab;
    public ImageButton imageButtonPrefab;
    public TextExtended textFieldPrefab;
    //canvas
    public RectTransform mainCanvas;
    public RectTransform overlayCanvas;
    //turn order list
    public RectTransform turnOrderButtonParent;
    //comp hotkeys
    public GameObject compHotkeysPanel;
    public RectTransform compHotkeysParent;
    //comp selection panel
    public GameObject openCompSelectionPanel;
    public GameObject closedCompSelectPanel;
    //stats panel
    public GameObject statsPanel;
    public Text powerText;
    public Text moveCostText;
    //move UI
    public GameObject moveUI;
    public Text moveDistance;
    public Text movePowerCost;
    //Cursors
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    //tooltip
    public TextExtended tooltip;
}

#endregion AdditionalStructs
public class CombatSystemInterface : Singleton<CombatSystemInterface>
{
    #region Fields
    //EditorExposed
    [SerializeField]
    private CombatGUIFields guiFields;

    //internal
    private Dictionary<TurnBasedUnit, TextExtended> unit_buttonRect_table = new Dictionary<TurnBasedUnit, TextExtended>();
    private List<ImageButton> compButtons = new List<ImageButton>();
    private Vector2 attackCursorOffset, defaultCursorOffset;
    //references
    private RectTransform moveCostUITrans;
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    #region GUISetup
    /// <summary>
    /// Add a ship to the turn order list
    /// </summary>
    /// <param name="unit"></param>
    public void AddShipButton(TurnBasedUnit unit)
    {
        //ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
        //buttonClone.SetText(unit.ShipBPMetaData.BlueprintName);
        //buttonClone.transform.SetParent(guiFields.turnOrderButtonParent, false);
        //unit_buttonRect_table.Add(unit, buttonClone);

        TextExtended textClone = Instantiate(guiFields.textFieldPrefab) as TextExtended;
        textClone.SetText( unit.ShipBPMetaData.BlueprintName);
        textClone.transform.SetParent(guiFields.turnOrderButtonParent, false);
        unit_buttonRect_table.Add(unit, textClone);
    }
    /// <summary>
    /// Shows buttons along the bottom to activate all components of a type at once. Pass in null to remove the buttons from the screen.
    /// </summary>
    /// <param name="activationMethod"></param>
    /// <param name="components"></param>
    public void ShowComponentHotkeyButtons(UnityAction<Type> activationMethod, IEnumerable<ShipComponent> components)
    {
        for (int i = compButtons.Count - 1; i >= 0; i--)
        {
            Destroy(compButtons[i].gameObject);
            compButtons.RemoveAt(i);
        }
        if (components == null || activationMethod==null)
        {
            guiFields.compHotkeysPanel.SetActive(false);
            return;
        }
        guiFields.compHotkeysPanel.SetActive(true);
        foreach (Type type in components.Select(c => c.GetType()).Distinct())
        {
            Type currentType = type;
            ShipComponent component = components.First(c => c.GetType() == currentType);
            ImageButton buttonClone = Instantiate(guiFields.imageButtonPrefab) as ImageButton;
            buttonClone.SetImage(component.MultipleSprite);
            buttonClone.AddOnClickListener(() => activationMethod(currentType));
            //buttonClone.AddOnPointerEnterListener(()=>{Debug.Log("Mouse over "+component.componentName);});
            //buttonClone.AddOnPointerExitListener(() => { Debug.Log("Mouse exit " + component.componentName); });
            compButtons.Add(buttonClone);
            buttonClone.transform.SetParent(guiFields.compHotkeysParent, false);
        }
    }//ShowComponentActivationButtons

    public void ShowToolTip(string toolTipText, Camera viewCamera, Vector3 worldPos)
    {
        guiFields.tooltip.gameObject.SetActive(true);
        guiFields.tooltip.SetText(toolTipText);
        //Vector2 viewPortPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        //Vector2 viewPortPos = viewCamera.WorldToScreenPoint(worldPos);
        //guiFields.tooltip.Trans.position = viewPortPos;
        guiFields.tooltip.RectTrans.anchoredPosition = worldPos.GetVector2() - guiFields.mainCanvas.sizeDelta * 0.5f;
    }
    public void HideTooltip()
    {
        guiFields.tooltip.gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows a UI element including the distance and power cost passed in
    /// </summary>
    /// <param name="position"></param>
    /// <param name="distance"></param>
    /// <param name="powerCost"></param>
    public void ShowMoveCostUI(Vector3 position , float distance, float powerCost)
    {
        Vector2 viewPortPos = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
        //moveCostUITrans.anchorMin = viewPortPos;
        //moveCostUITrans.anchorMax = viewPortPos;
        moveCostUITrans.anchoredPosition = viewPortPos - guiFields.mainCanvas.sizeDelta*0.5f;
        //moveCostUITrans.position = position;
        guiFields.moveDistance.text = distance.ToString("0 u");
        guiFields.movePowerCost.text = powerCost.ToString("0") ;
        guiFields.moveUI.SetActive(true);
    }
    /// <summary>
    /// Deactivates the movement UI
    /// </summary>
    public void HideMoveUI()
    {
        guiFields.moveUI.SetActive(false);
    }

    public void UpdateStats(float currentPower, float moveCost)
    {
        //Debug.Log("Current power = " + unit.CurrentPower);
        guiFields.powerText.text = currentPower.ToString();
        guiFields.moveCostText.text = moveCost.ToString("0.00");
    }
    public void SetPowerValid(bool valid=true)
    {
        if(valid)
        {
            guiFields.powerText.color = Color.white;
        }
        else
        {
            guiFields.powerText.color = Color.red;
        }
    }
    /// <summary>
    /// Shows the power to be displayed on the GUI
    /// </summary>
    /// <param name="currentPower"></param>
    //public void ShowPower(float currentPower)
    //{
    //    guiFields.powerText.text = currentPower.ToString();
    //}
    #endregion GUISetup
    
    public void ShowAttackCursor(bool show)
    {
        if(show)
        {
            Cursor.SetCursor(guiFields.attackCursor,attackCursorOffset , CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(guiFields.defaultCursor, defaultCursorOffset, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Shows the units in the turn order list, in the order that it was passed in (top to bottom)
    /// </summary>
    /// <param name="units"></param>
    public void UpdateTurnOrderPanel(List<TurnBasedUnit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            TextExtended button = unit_buttonRect_table[units[i]];
            //button.SetText( units[i].ShipBPMetaData.BlueprintName);
            button.RectTrans.SetSiblingIndex(i);
        }
    }
    /// <summary>
    /// Shows/Hides the Component Selection Panel for the player to select which components to activate 
    /// </summary>
    /// <param name="show"></param>
    public void ShowComponentSelectionPanel(bool show)
    {
        guiFields.openCompSelectionPanel.SetActive(show);
        guiFields.closedCompSelectPanel.SetActive(!show);
        TurnBasedCombatSystem.Instance.ShowingSelectionPanel(show);
    }
    /// <summary>
    /// Enables the hidden component selection panel that the player can mouse over to reveal the full panel
    /// </summary>
    /// <param name="show"></param>
    public void EnableComponentSelectionPanel(bool show)
    {
        guiFields.closedCompSelectPanel.SetActive(show);
        guiFields.openCompSelectionPanel.SetActive(false);
        if(!show)
        {
            TurnBasedCombatSystem.Instance.ShowingSelectionPanel(false);
        }
    }
    /// <summary>
    /// Show/Hide a panel to display the target's components over
    /// </summary>
    /// <param name="show"></param>
    /// <param name="targetName"></param>
    //public void ShowTargetingPanel(bool show, string targetName)
    //{
    //    guiFields.targetedShipName.text = targetName;
    //    guiFields.targetingPanel.SetActive(show);
    //}
    /// <summary>
    /// Show/Hide the stats panel
    /// </summary>
    /// <param name="show"></param>
    public void ShowStatsPanel(bool show)
    {
        guiFields.statsPanel.SetActive(show);
    }

    #endregion PublicMethods

    private void Awake()
    {
        moveCostUITrans = (RectTransform)guiFields.moveUI.transform;
        #if FULL_DEBUG
        if(!moveCostUITrans)
        {
            Debug.LogError("MoveUICanvas not found");
        }
        #endif
        HideMoveUI();
        attackCursorOffset = new Vector2(guiFields.attackCursor.width * 0.5f, guiFields.attackCursor.height * 0.5f);
        defaultCursorOffset = new Vector2(guiFields.defaultCursor.width * 0.1f, guiFields.defaultCursor.height * 0.1f);
        ShowAttackCursor(false);
    }
    


    #endregion Methods
}
