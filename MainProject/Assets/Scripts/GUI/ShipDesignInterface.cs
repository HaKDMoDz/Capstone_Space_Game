using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

#region AdditionalStructs
[Serializable]
public struct InterfaceGUI_Fields
{
    public RectTransform hullButtonParent;
    public RectTransform compButtonParent;
    public ButtonWithContent buttonPrefab;
}
#endregion AdditionalStructs
public class ShipDesignInterface : Singleton<ShipDesignInterface>
{

    #region Fields
    #region EditorExposed
    [SerializeField]
    private InterfaceGUI_Fields guiFields;
    #endregion EditorExposed
    #endregion Fields

    #region Methods
    #region Private
    #region UnityCallbacks
    private void Start()
    {
        SetupGUI();
    }
    #endregion UnityCallbacks
    #region GUI
    private void SetupGUI()
    {
        //Hull buttons
        //foreach (var id_hull in ShipDesignSystem.Instance.id_hull_table)
        foreach(var id_hull in HullTable.id_hull_table)
        {
            ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
            buttonClone.transform.SetParent(guiFields.hullButtonParent,false); 
            buttonClone.buttonText.text = id_hull.Value.hullName;
            int hull_ID = id_hull.Key;
            buttonClone.button.onClick.AddListener(() =>
            {
                BuildHull(hull_ID);
            });
        }
        //Component Buttons
        //foreach (var id_comp in ShipDesignSystem.Instance.id_comp_table)
        foreach (var id_comp in  ComponentTable.id_comp_table)
        {
            ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
            buttonClone.transform.SetParent(guiFields.compButtonParent, false);
            buttonClone.buttonText.text = id_comp.Value.componentName;
            int compID = id_comp.Key;
            buttonClone.button.onClick.AddListener(() =>
                {
                    SelectComponentToBuild(compID);
                });
        }
    }
    #endregion GUI

    private IEnumerator StartPlacementSequence(ShipComponent selectedComponent)
    {
        bool runSequence = true;
        bool dragging = false;
        RaycastHit hit;
        int componentSlotLayer = GlobalTagsAndLayers.Instance.layers.componentSlotLayer;
        yield return null;

        while(runSequence)
        {
            if(Input.GetMouseButtonDown(0) || dragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 1000.0f, 1<<componentSlotLayer))
                {
                    dragging = true;
                    ComponentSlot slot = hit.transform.GetComponent<ComponentSlot>();
                    ShipDesignSystem.Instance.BuildComponent(slot, selectedComponent);
                }
            }
            if(Input.GetMouseButtonUp(0)||Input.GetKeyDown(KeyCode.Escape))
            {
                runSequence = false;
            }
            yield return null;
        }
    }

    #endregion Private
    
    #region Public
    #region GUI
    public void SelectComponentToBuild(int componentID)
    {
        #if FULL_DEBUG
        Debug.Log("Selected component: " + ComponentTable.GetComp(componentID).componentName);
        #endif
        StartCoroutine(StartPlacementSequence(ComponentTable.GetComp(componentID)));
    }
    #endregion GUI

    #region DesignSystemAccess
    public void BuildHull(int hull_ID)
    {
        ShipDesignSystem.Instance.BuildHull(hull_ID);
    }
    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {
        ShipDesignSystem.Instance.BuildComponent(slot, component);
    }
    public void SaveBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.SaveBlueprint(fileName);
    }
    public void LoadBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.LoadBlueprint(fileName);
    }
    public void DeleteBlueprint(string fileName)
    {
        ShipDesignSystem.Instance.DeleteBlueprint(fileName);
    }
    public void DeleteAllBlueprints()
    {
        ShipDesignSystem.Instance.DeleteAllBlueprints();
    }
    public void ClearScreen()
    {
        ShipDesignSystem.Instance.ClearScreen();
    }
    #endregion DesignSystemAccess
    #endregion Public
    #endregion Methods

}
