/*
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
    public RectTransform turnOrderHeader;
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
    public Animator powerTextAnim;
    public float textTweenSpeed;
    //move UI
    public GameObject moveUI;
    public Text moveDistance;
    public Text movePowerCost;
    //Cursors
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public Texture2D invalidCursor;
    //tooltip
    public TextExtended tooltip;
    //Player ship mode buttons
    public GameObject modeButtons;
    public Button moveButton;
    public Button tacticalButton;
    public Button endTurnButton;
    //floating damage
    public TextExtended floatingDamagePrefab;
}
public enum CursorType { Default, Attack, Invalid }
#endregion AdditionalStructs
public class CombatSystemInterface : Singleton<CombatSystemInterface>
{
    #region Fields
    //EditorExposed
    [SerializeField]
    private CombatGUIFields guiFields;
    [SerializeField]
    private float textTweenSpeed = 5.0f;


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
        TextExtended textClone = Instantiate(guiFields.textFieldPrefab) as TextExtended;
        textClone.AddOnPointerEnterListener(() => unit.ShowHPBars(true));
        textClone.AddOnPointerExitListener(() => unit.ShowHPBars(false));
        textClone.SetText( unit.ShipBPMetaData.BlueprintName);
        textClone.transform.SetParent(guiFields.turnOrderButtonParent, false);
        unit_buttonRect_table.Add(unit, textClone);
    }
    /// <summary>
    /// Shows buttons along the bottom to activate all components of a type at once. Pass in null to remove the buttons from the screen.
    /// </summary>
    /// <param name="activate"></param>
    /// <param name="component_valid_table"></param>
    public void ShowComponentHotkeyButtons(UnityAction<Type> activate, Dictionary<ShipComponent, bool> component_valid_table)
    {
        for (int i = compButtons.Count - 1; i >= 0; i--)
        {
            Destroy(compButtons[i].gameObject);
            compButtons.RemoveAt(i);
        }
        if (component_valid_table == null || activate == null)
        {
            guiFields.compHotkeysPanel.SetActive(false);
            return;
        }
        guiFields.compHotkeysPanel.SetActive(true);
        foreach (Type type in component_valid_table.Keys.Select(comp=>comp.GetType()).Distinct())
        {
            Type currentType = type;
            ShipComponent component = component_valid_table.Keys.First(comp=>comp.GetType()==currentType);
            ImageButton buttonClone = Instantiate(guiFields.imageButtonPrefab) as ImageButton;
            buttonClone.SetImage(component.MultipleSprite);
            if (component_valid_table[component])
            {
                buttonClone.AddOnClickListener(() => activate(currentType));
            }
            else
            {
                SetInvalidButton(buttonClone);
            }
            compButtons.Add(buttonClone);
            buttonClone.transform.SetParent(guiFields.compHotkeysParent, false);
        }
    }//ShowComponentActivationButtons
    private void SetInvalidButton(ImageButton button)
    {
        button.Button.interactable = false;
        button.Image.color = button.Image.color.WithAplha(0.25f);
    }
    public void ShowFloatingDamage(float damage, Vector3 worldPos, Color textColour)
    {
        TextExtended floatingDmgClone = (TextExtended)Instantiate(guiFields.floatingDamagePrefab);
        floatingDmgClone.SetText(damage.ToString());
        floatingDmgClone.SetTextColour(textColour);
        RectTransform textTrans = (RectTransform)floatingDmgClone.transform;
        textTrans.SetParent(guiFields.overlayCanvas.transform, false);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);
        textTrans.position = screenPos.ToVector3();
    }
    public void ShowToolTip(string toolTipText, Vector3 screenPos)
    {
        guiFields.tooltip.gameObject.SetActive(true);
        guiFields.tooltip.SetText(toolTipText);
        guiFields.tooltip.RectTrans.anchoredPosition = screenPos.ToVector2() - guiFields.overlayCanvas.sizeDelta * 0.5f;
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

    float targetValue;
    public void UpdateStats(float currentPower, float moveCost, bool tween)
    {
        //Debug.Log("Current power = " + unit.CurrentPower);
        if (tween)
        {
            //StartCoroutine(TweenTextNumber(guiFields.powerText, currentPower));
            //StopCoroutine("TweenTextNumber");
            //guiFields.powerText.text = currentPower.ToString();
            targetValue = currentPower;
            StartCoroutine(TweenTextNumber());
        }
        else
        {
            StopAllCoroutines();
            guiFields.powerText.text = currentPower.ToString();
        }
        guiFields.moveCostText.text = moveCost.ToString("0.00");
    }
    //private IEnumerator TweenTextNumber(Text textField, float targetValue)
    private IEnumerator TweenTextNumber()
    {
#if FULL_DEBUG
        float currentValue;
        if(!float.TryParse(guiFields.powerText.text, out currentValue))
        {
            Debug.LogError("Could not parse text " + guiFields.powerText.name + " to float");
        }
#else
        float currentValue = float.Parse(guiFields.powerText.text);
#endif
        //float increment = targetValue > currentValue ? textTweenSpeed : -textTweenSpeed;
        //Debug.Log("Current value: " + currentValue + " target " + targetValue + " diff " + Mathf.Abs(currentValue - targetValue));
        //while (Mathf.Abs(currentValue - targetValue)>textTweenSpeed)
        while (Mathf.Abs(currentValue - targetValue) > GlobalVars.LerpDistanceEpsilon)
        {
            //currentValue += increment;
            currentValue = Mathf.Lerp(currentValue, targetValue, textTweenSpeed * Time.deltaTime);
            guiFields.powerText.text = currentValue.ToString("0");
            yield return null;
        }
        guiFields.powerText.text = targetValue.ToString();
    }
    public void SetPowerValid(bool valid=true)
    {
        if(valid)
        {
            guiFields.powerText.color = Color.white;
        }
        else
        {
            StopAllCoroutines();
            guiFields.powerText.color = Color.red;
            guiFields.powerTextAnim.SetTrigger("Blink");
            StartCoroutine(SetPowerBackToValidAfterAnim());
        }
    }
    private IEnumerator SetPowerBackToValidAfterAnim()
    {
        yield return new WaitForSeconds(0.75f);
        SetPowerValid();
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

    public void ShowModeButtons(bool show)
    {
        guiFields.modeButtons.SetActive(show);
        EnableMoveButton(false);
        EnableTacticalButton(false);
    }
    public void EnableMoveButton(bool enable, UnityAction action = null)
    {
        guiFields.moveButton.onClick.RemoveAllListeners();
        if (enable)
        {
            #if FULL_DEBUG
            if(action == null)
            {
                Debug.LogError("Action is null");
            }
            #endif
            guiFields.moveButton.onClick.AddListener(action);
        }
        guiFields.moveButton.gameObject.SetActive(enable);
    }
    public void EnableTacticalButton(bool enable, UnityAction action=null)
    {
        guiFields.tacticalButton.onClick.RemoveAllListeners();
        if(enable)
        {
            #if FULL_DEBUG
            if(action == null)
            {
                Debug.LogError("Action is null");
            }
            #endif
            guiFields.tacticalButton.onClick.AddListener(action);
        }
        guiFields.tacticalButton.gameObject.SetActive(enable);
    }
    public void SetEndTurnEvent(UnityAction action)
    {
        guiFields.endTurnButton.onClick.RemoveAllListeners();
        guiFields.endTurnButton.onClick.AddListener(action);
    }
    public void SetCursorType(CursorType type)
    {
        switch (type)
        {
            case CursorType.Default:
                Cursor.SetCursor(guiFields.defaultCursor, defaultCursorOffset, CursorMode.Auto);
                break;
            case CursorType.Attack:
                Cursor.SetCursor(guiFields.attackCursor,attackCursorOffset , CursorMode.Auto);
                break;
            case CursorType.Invalid:
                Cursor.SetCursor(guiFields.invalidCursor, attackCursorOffset, CursorMode.Auto);
                break;
        }
    }
    /// <summary>
    /// Shows the units in the turn order list, in the order that it was passed in (top to bottom)
    /// </summary>
    /// <param name="units"></param>
    public void UpdateTurnOrderPanel(List<TurnBasedUnit> units, bool reset)
    {
        if (reset)
        {
            foreach (var unit_buttonRect in unit_buttonRect_table)
            {
                Destroy(unit_buttonRect.Value.gameObject);
            }
            unit_buttonRect_table.Clear();
            foreach (var unit in units)
            {
                AddShipButton(unit);
            }
        }
        else
        {
            for (int i = 0; i < units.Count; i++)
            {
                Debug.Log("Unit: " + units[i].ShipBPMetaData.BlueprintName + " time left " + units[i].TimeLeftToTurn);
                TextExtended button = unit_buttonRect_table[units[i]];
                //button.SetText( units[i].ShipBPMetaData.BlueprintName);
                button.RectTrans.SetSiblingIndex(i);
                
            }
            guiFields.turnOrderHeader.SetSiblingIndex(0);
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
        SetCursorType(CursorType.Default);
    }
    


    #endregion Methods
}
