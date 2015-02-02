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
    public ButtonWithContent buttonPrefab;
    public RectTransform turnOrderButtonParent;
    public RectTransform compButtonParent;
    public GameObject openCompSelectionPanel;
    public GameObject closedCompSelectPanel;

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
    private List<ButtonWithContent> compButtons = new List<ButtonWithContent>();
    #endregion Fields

    #region Methods

    #region PublicMethods
    
    #region GUISetup
    public void AddShipButton(TurnBasedUnit unit)
    {
        ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
        buttonClone.buttonText.text = unit.shipBPMetaData.blueprintName;
        buttonClone.transform.SetParent(guiFields.turnOrderButtonParent, false);
        unit_buttonRect_table.Add(unit, buttonClone);
    }
    public void ShowComponentActivationButtons(UnityAction<Type> activationMethod, List<ShipComponent> components)
    {
        for (int i = compButtons.Count - 1; i >= 0; i--)
        {
            Destroy(compButtons[i].gameObject);
            compButtons.RemoveAt(i);
        }

        if (components == null)
        {
            return;
        }
   
        foreach (Type type in components.Select(c => c.GetType()).Distinct())
        {
            Type currentType = type;
            ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
            buttonClone.buttonText.text = components.First(c => c.GetType() == currentType).componentName;
            buttonClone.button.onClick.AddListener(() => activationMethod(currentType));
            compButtons.Add(buttonClone);
            buttonClone.transform.SetParent(guiFields.compButtonParent, false);
        }
    }
    #endregion GUISetup

    public void UpdateTurnOrderPanel(List<TurnBasedUnit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            ButtonWithContent button = unit_buttonRect_table[units[i]];
            button.buttonText.text = units[i].shipBPMetaData.blueprintName;
            button.transform.SetSiblingIndex(i);
        }
    }

    public void ShowComponentSelectionPanel(bool show)
    {
        guiFields.openCompSelectionPanel.SetActive(show);
        guiFields.closedCompSelectPanel.SetActive(!show);

        TurnBasedCombatSystem.Instance.ShowingSelectionPanel(show);
    }
    public void EnableComponentSelectionPanel(bool show)
    {
        guiFields.closedCompSelectPanel.SetActive(show);
        guiFields.openCompSelectionPanel.SetActive(false);
        if(!show)
        {
            TurnBasedCombatSystem.Instance.ShowingSelectionPanel(false);
        }
    }

    

    #endregion PublicMethods

    #region PrivateMethods

    #region GUIRelated

    //private IEnumerator ShowCompSelectionPanel(bool show)
    //{
    //    Debug.Log("opening Panel");

    //    float destRightValue;
    //    if(show)
    //    {
    //        destRightValue = guiFields.openRightSelectionPanel;
    //    }
    //    else
    //    {
    //        destRightValue = guiFields.closedRightSelectionPanel;
    //    }
    //    float time = 0.0f;

    //    Rect panelRect = guiFields.compSelectionPanelRect.rect;
    //    while (time < 1.0f)
    //    {
    //        //guiFields.compSelectionPanelRect.rect.Set(guiFields.compSelectionPanelRect.rect.xMin,
    //        //    guiFields.compSelectionPanelRect.rect.yMin,
    //        //    Mathf.Lerp(guiFields.compSelectionPanelRect.rect.width, destRightValue, time),
    //        //    panelRect.height);

    //        //Vector2 rectSize = guiFields.compSelectionPanelRect.sizeDelta;
    //        //rectSize.x = Mathf.Lerp(rectSize.x, destRightValue, time);
    //        //guiFields.compSelectionPanelRect.sizeDelta = rectSize;
    //        /*guiFields.compSelectionPanelRect.position = Vector2.zero;*/
    //        float rightPos = Mathf.Lerp(guiFields.compSelectionPanelRect.Size().x, destRightValue, time);
    //        guiFields.compSelectionPanelRect.SetWidth(rightPos);

    //        time += Time.deltaTime / guiFields.changeTimeSelectionPanel;

    //        yield return null;
    //    }



    //}
    
    #endregion GUIRelated

    #region UnityCallbacks

    private void Awake()
    {
        unit_buttonRect_table = new Dictionary<TurnBasedUnit, ButtonWithContent>();
    }

    #endregion UnityCallbacks

    #endregion PrivateMethods

    #endregion Methods
}
