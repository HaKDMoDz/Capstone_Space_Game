﻿/*
  ShipDesignInterface.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 22/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
#endregion Usings

public class ShipDesignInterface : Singleton<ShipDesignInterface>
{
    private enum CursorType { Default, Eraser };
    #region Fields
    //Editor Exposed
    //Prefabs
    [SerializeField]
    private ButtonWithContent buttonPrefab;
    [SerializeField]
    private Button_TextAndImage button_TextAndImagePrefab;
    [SerializeField]
    private TextExtended iconCorvette;
    [SerializeField]
    private TextExtended iconFrigate;
    [SerializeField]
    private HeaderText headerPrefab;
    [SerializeField]
    private GameObject separatorPrefab;
    //building panels
    [SerializeField]
    private Animator compPanelAnim;
    [SerializeField]
    private Animator hullPanelAnim;
    [SerializeField]
    private RectTransform hullButtonParent;
    [SerializeField]
    private RectTransform compButtonParent;
    //saving
    [SerializeField]
    private InputDialogueBox saveDialogueBox;
    //loading
    [SerializeField]
    private GameObject loadPanel;
    [SerializeField]
    private RectTransform loadButtonParent;
    //stats
    [SerializeField]
    private Animator statsPanelAnim;
    [SerializeField]
    private ShipStatsPanel statsPanel;
    //modal box
    [SerializeField]
    private ModalPanel modalPanel;
    //Save button
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Image saveButtonImage;
    //cursors
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Texture2D eraserCursor;
#if FULL_DEBUG
    [SerializeField]
    private bool showAI_Hulls=false;
#endif
    //Internal
    private Dictionary<string, GameObject> blueprintName_button_table;
    private string selectedBP;
    private ShipComponent selectedComp;
    //References
    ShipDesignSystem shipDesignSystem;
    #endregion Fields


    #region Methods
    #region Private

    #region UnityCallbacks

    private void Awake()
    {
        blueprintName_button_table = new Dictionary<string, GameObject>();
        shipDesignSystem = ShipDesignSystem.Instance;
        AllowSaving(false);
    }
    #endregion UnityCallbacks

    #region GUIBuilders
    /// <summary>
    /// Sets up all dynamic elements in the GUI like buttons for Hulls and Components based on the database entries and Load buttons based on saved blueprints
    /// </summary>
    private void SetupGUI()
    {
        //Setup Hull Buttons
#if FULL_DEBUG
        Dictionary<int, Hull> hullTable = showAI_Hulls ? HullTable.id_hull_table : HullTable.id_hull_table.Where(h => h.Value.PlayerAccessible).ToDictionary(h => h.Key, h => h.Value);
        foreach (var id_hull in hullTable)
#else
        foreach (var id_hull in HullTable.id_hull_table.Where(h=>h.Value.PlayerAccessible))
#endif
        {
            //create the button and position it
            ButtonWithContent buttonClone = Instantiate(buttonPrefab) as ButtonWithContent;
            buttonClone.transform.SetParent(hullButtonParent, false);
            buttonClone.SetText(id_hull.Value.hullName);
            int hull_ID = id_hull.Key;
            buttonClone.AddOnClickListener(() =>
            {
                //the method to call when the button is clicked
                BuildHull(hull_ID);
            });
        }

        SetupComponentButtons();

        //Add blueprint buttons to Load Panel and Fleet Panel
        foreach (string blueprintName in shipDesignSystem.GetSaveFileList())
        {
            AddBlueprintButton(blueprintName);
        }
        //Add current fleet buttons
        //foreach (string blueprintName in FleetManager.Instance.CurrentFleet.Select(meta => meta.BlueprintName))
        //{
        //    AddCurrentFleetButton(blueprintName);
        //}
        //fleetStrBar.SetValue((float)(FleetManager.Instance.CurrentFleetStrength) / (float)(FleetManager.Instance.MaxFleetStrength));

        saveDialogueBox.Setup((fileName) => SaveBlueprint(fileName), () => ShowSaveDialogueBox(false));

    }
    private void SetupComponentButtons()
    {
        var weapons = ComponentTable.GetShipComponentsOfType(ComponentType.Weapon);
        var defenses = ComponentTable.GetShipComponentsOfType(ComponentType.Defense);
        var engineering = ComponentTable.GetShipComponentsOfType(ComponentType.Engineering);
        var supports = ComponentTable.GetShipComponentsOfType(ComponentType.Support);

        if (weapons.Count() > 0)
        {
            AddHeader(ComponentType.Weapon);
            AddComponentButtons(weapons);
            GameObject sep = Instantiate(separatorPrefab) as GameObject;
            sep.transform.SetParent(compButtonParent, false);
        }
        if (defenses.Count() > 0)
        {
            AddHeader(ComponentType.Defense);
            AddComponentButtons(defenses);
            GameObject sep = Instantiate(separatorPrefab) as GameObject;
            sep.transform.SetParent(compButtonParent, false);
        }
        if (engineering.Count() > 0)
        {
            AddHeader(ComponentType.Engineering);
            AddComponentButtons(engineering);
            GameObject sep = Instantiate(separatorPrefab) as GameObject;
            sep.transform.SetParent(compButtonParent, false);
        }
        if (supports.Count() > 0)
        {
            AddHeader(ComponentType.Support);
            AddComponentButtons(supports);
        }
    }
    private void AddHeader(ComponentType type)
    {
        HeaderText headerClone = Instantiate(headerPrefab) as HeaderText;
        headerClone.transform.SetParent(compButtonParent, false);
        headerClone.SetText(type.ToString());
    }
    private void AddComponentButtons(IEnumerable<ShipComponent> components)
    {
        foreach (var comp in components)
        {
            Button_TextAndImage buttonClone = Instantiate(button_TextAndImagePrefab) as Button_TextAndImage;
            buttonClone.transform.SetParent(compButtonParent, false);
            buttonClone.SetText(comp.componentName);
            buttonClone.SetSprite(comp.SingleSprite);
            ShipComponent tempCompVar = comp;
            buttonClone.AddOnClickListener(() => SelectComponentToBuild(tempCompVar));
        }
    }
    /// <summary>
    /// Helper method to add a blueprint button to the load panel and the fleet panel
    /// </summary>
    /// <param name="fileName">
    /// the fileName that the button will correspond to
    /// </param>
    private void AddBlueprintButton(string fileName)
    {
        ButtonWithContent loadButtonClone = Instantiate(buttonPrefab) as ButtonWithContent;
        loadButtonClone.transform.SetParent(loadButtonParent, false);
        loadButtonClone.SetText(fileName);
        loadButtonClone.AddOnClickListener(() =>
            {
                loadButtonClone.Button.Select();
                SelectBlueprintToLoad(fileName);
            });
        blueprintName_button_table.Add(fileName, loadButtonClone.gameObject);
    }
    /// <summary>
    /// Removes a blueprint button
    /// </summary>
    /// <param name="fileName"></param>
    private void RemoveBlueprintButton(string fileName)
    {
#if !NO_DEBUG
        if (!blueprintName_button_table.ContainsKey(fileName))
        {
            Debug.LogError("Blueprint " + fileName + " does not exist in button table");
            return;
        }
        GameObject button;
        if (!blueprintName_button_table.TryGetValue(fileName, out button))
        {
            Debug.LogError("No buttons found for blueprint " + fileName);
            return;
        }
        else
        {
            Destroy(button);
            blueprintName_button_table.Remove(fileName);
        }
        
#else //NO_DEBUG
        Destroy(blueprintName_button_table[fileName]);
        blueprintName_button_table.Remove(fileName);
#endif
    }
    #endregion GUIBuilders
    private void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Eraser:
                Cursor.SetCursor(eraserCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                break;
        }
    }
    public void StopBuilding()
    {
        StopCoroutine("StartPlacementSequence");
    }
    /// <summary>
    /// Lets the user place components on the ship being built.
    /// The user can drag with the mouse to place multiple components of the same type
    /// </summary>
    /// <param name="selectedComponent">
    /// The component to build
    /// </param>
    private IEnumerator StartPlacementSequence()
    {
        if (selectedComp)
        {
            while (true)
            {
                if (Input.GetMouseButton((int)MouseButton.Left))
                {
                    ComponentSlot slot;
                    if (GetSlotAtMouse(out slot))
                    {
                        shipDesignSystem.BuildComponent(slot, selectedComp);
                    }
                }
                yield return null;
            }
        }
    }
    private bool GetSlotAtMouse(out ComponentSlot slot)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1 << TagsAndLayers.ComponentSlotLayer))
        {
            slot = hit.collider.GetComponent<ComponentSlot>();
            return true;
        }
        else
        {
            slot = null;
            return false;
        }        
    }
    /// <summary>
    /// Removes all GUI elements from the screen
    /// </summary>
    private void ClearGUI()
    {
        ShowSaveDialogueBox(false);
        ShowLoadPanel(false);
        FleetInterface.Instance.ShowFleetPanel(false);
        //ShowStatsPanel(false);
    }
    private void ShowModalPanel(bool show)
    {
        StopCoroutine("StartPlacementSequence");
        modalPanel.gameObject.SetActive(show);
    }

    #region Helper

    //private void ClearCurrentFleet()
    //{
    //    foreach (Transform child in currentFleetParent)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    FleetManager.Instance.CurrentFleet.Clear();
    //}
    #endregion Helper
    #endregion Private

    #region Public
    public void Init()
    {
        SetupGUI();
        InputManager.Instance.RegisterMouseButtonsHold(RemoveComponent, MouseButton.Right);
        InputManager.Instance.RegisterMouseButtonsUp((button) => SetCursor(CursorType.Default), MouseButton.Right);
    }
    #region GUIAccess

    public void SelectBlueprintToLoad(string bpName)
    {
#if FULL_DEBUG
        Debug.Log("Selected " + bpName);
#endif
        selectedBP = bpName;
    }
    public void DeleteSelectedBP()
    {
        if (String.IsNullOrEmpty(selectedBP))
        {
            return;
        }
        ShowModalPanel(true);
        modalPanel.ShowMessageWithYesCancel("Delete blueprint: " + selectedBP + " ?",
            () =>
            {
#if FULL_DEBUG
                Debug.Log("Delete selected BP: " + selectedBP);
#endif
                DeleteBlueprint(selectedBP);
                selectedBP = "";
            },
            null);
    }
    public void LoadSelectedBP()
    {
        if (String.IsNullOrEmpty(selectedBP))
        {
            return;
        }
#if FULL_DEBUG
        Debug.Log("Load selected BP: " + selectedBP);
#endif
        LoadBlueprint(selectedBP);
        selectedBP = "";
    }

    /// <summary>
    /// Selects the component to build and starts the placement sequence
    /// </summary>
    /// <param name="componentID"></param>
    public void SelectComponentToBuild(int componentID)
    {
        if (!shipDesignSystem.buildingShip)
        {
#if FULL_DEBUG
            Debug.Log("Not building a ship");
#endif
            return;
        }
#if FULL_DEBUG
        Debug.Log("Selected component: " + ComponentTable.GetComp(componentID).componentName);
#endif
        SelectComponentToBuild(ComponentTable.GetComp(componentID));
    }
    public void SelectComponentToBuild(ShipComponent component)
    {
        if (!shipDesignSystem.buildingShip)
        {
#if FULL_DEBUG
            Debug.Log("Not building a ship");
#endif
            return;
        }
#if FULL_DEBUG
        Debug.Log("Selected component: " + component.componentName);
#endif
        selectedComp = component;
        StopCoroutine("StartPlacementSequence");
        StartCoroutine("StartPlacementSequence");
    }

    /// <summary>
    /// Shows the Save Dialogue Box for the user to enter the name of the blueprint to save as
    /// </summary>
    /// <param name="show"></param>
    public void ShowSaveDialogueBox(bool show)
    {
        StopCoroutine("StartPlacementSequence");
        if (!show)
        {
            saveDialogueBox.gameObject.SetActive(false);
        }
        //showing dialogue box
        else if (shipDesignSystem.buildingShip)
        {
            saveDialogueBox.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(saveDialogueBox.InputFieldEx.gameObject);
            saveDialogueBox.GetInputField().ActivateInputField();
            saveDialogueBox.GetInputField().text = statsPanel.GetBlueprintName();
        }
#if FULL_DEBUG
        else
        {
            Debug.Log("No Ship being built");
        }
#endif

    }
    /// <summary>
    /// Shows the panel that lists all saved blueprints so the user can load one of them
    /// </summary>
    /// <param name="show"></param>
    public void ShowLoadPanel(bool show)
    {
        StopCoroutine("StartPlacementSequence");
        loadPanel.SetActive(show);
    }

    /// <summary>
    /// Shows a panel to build and manage the fleet to take into combat
    /// </summary>
    /// <param name="show"></param>
    //public void ShowFleetPanel(bool show)
    //{
    //    fleetPanel.SetActive(show);
    //}

    public void ShowStatsPanel(bool show)
    {
        if (show)
        {
            statsPanelAnim.enabled = true;
            statsPanelAnim.Play("PanelMoveInRight");
        }
        else
        {
            statsPanelAnim.Play("PanelMoveOutRight");
        }
    }
    private void ShowHullPanel()
    {
        compPanelAnim.Play("CompPanelMoveOut");
        hullPanelAnim.Play("CompPanelMoveIn");
    }
    private void ShowComponentPanel()
    {
        compPanelAnim.enabled = true;
        compPanelAnim.Play("CompPanelMoveIn");
        hullPanelAnim.enabled = true;
        hullPanelAnim.Play("CompPanelMoveOut");
    }
    /// <summary>
    /// Saves the current fleet
    /// </summary>
    /// <summary>
    /// Updates the stats displayed in the stat panel based on the meta data for the blueprint being built
    /// </summary>
    /// <param name="shipBPMetaData"></param>
    public void UpdateStatsPanel(ShipBlueprintMetaData shipBPMetaData)
    {
        if (!statsPanel.gameObject.activeSelf)
        {
            ShowStatsPanel(true);
        }
        statsPanel.UpdateStats(shipBPMetaData.BlueprintName, shipBPMetaData.ExcessPower, shipBPMetaData.MoveCost, shipBPMetaData.ShieldStr);
    }
    public void AllowSaving(bool allow)
    {
        saveButton.interactable = allow;
        if (allow)
        {
            saveButtonImage.color = saveButtonImage.color.WithAplha(1.0f);
            TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ShipStats, false);
        }
        else
        {
            saveButtonImage.color = saveButtonImage.color.WithAplha(0.25f);
        }
    }
    #endregion GUIAccess

    #region DesignSystemAccess
    /// <summary>
    /// Calls the BuildHull method of the ShipDesignSystem
    /// </summary>
    /// <param name="hull_ID"></param>
    public void BuildHull(int hull_ID)
    {
        shipDesignSystem.BuildHull(hull_ID);
        ShowStatsPanel(true);
        ShowComponentPanel();
    }
    /// <summary>
    /// Calls the BuildComponent method of the ShipDesignSystem
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="component"></param>
    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {
        shipDesignSystem.BuildComponent(slot, component);
    }
    public void RemoveComponent(MouseButton button)
    {
        if (!shipDesignSystem.buildingShip) return;
        SetCursor(CursorType.Eraser);
        ComponentSlot slot;
        if(GetSlotAtMouse(out slot) && slot.InstalledComponent)
        {
            shipDesignSystem.RemoveComponent(slot);
        }
    }
    //GUI should not access directly
    /// <summary>
    /// Saves the current blueprint using the name entered in the SaveDialgoueBox
    /// Calls the SaveBlueprint method of the ShipDesignSystem
    /// </summary>
    /// <param name="fileName"></param>
    private void SaveBlueprint(string fileName)
    {
        Debug.Log("Saving " + fileName);
        if (shipDesignSystem.FileExists(fileName))
        {
            ShowModalPanel(true);
            modalPanel.ShowMessageWithYesCancel("A Blueprint named \"" + fileName + "\" already exists. Overwrite?",
                () =>
                {
                    shipDesignSystem.DeleteBlueprint(fileName);
                    shipDesignSystem.SaveBlueprint(fileName);
                    ClearGUI();
                },
                null);
        }
        else
        {
            AddBlueprintButton(fileName);
            FleetInterface.Instance.AddBlueprintButton(fileName);
            shipDesignSystem.SaveBlueprint(fileName);
            ClearGUI();
        }
    }
    //GUI should not access directly
    /// <summary>
    /// Loads the blueprint selected from the Load Panel. Calls the LoadBlueprint method of ShipDesignSystem
    /// </summary>
    /// <param name="fileName"></param>
    private void LoadBlueprint(string fileName)
    {
        ClearGUI();
        AllowSaving(false);
        shipDesignSystem.LoadBlueprint(fileName);
        ShowStatsPanel(true);
        ShowComponentPanel();
    }
    /// <summary>
    /// Calls DeleteBlueprint on the ShipDesignSystem
    /// </summary>
    /// <param name="fileName"></param>
    public void DeleteBlueprint(string fileName)
    {
        //if (FleetManager.Instance.CurrentFleetContains(shipDesignSystem.GetMetaData(fileName)))
        //{
        //    Debug.Log("Clearing Fleet as it includes " + fileName);
        //    ClearCurrentFleet();
        //}
        FleetInterface.Instance.OnDeleteBlueprint(fileName);
        shipDesignSystem.DeleteBlueprint(fileName);
        RemoveBlueprintButton(fileName);

    }
    /// <summary>
    /// Removes all Load Buttons from the Load Panel and calls DeleteAllBlueprints on the ShipDesignSystem
    /// </summary>
    public void DeleteAllBlueprints()
    {

        for (int i = blueprintName_button_table.Count - 1; i >= 0; i--)
        {
            RemoveBlueprintButton(blueprintName_button_table.ElementAt(i).Key);
        }
        //ClearCurrentFleet();
        ClearGUI();
        shipDesignSystem.DeleteAllBlueprints();
    }
    /// <summary>
    /// Clears the GUI and calls ClearScreen on the ShipDesignSystem
    /// </summary>
    public void ClearScreen()
    {
        ClearGUI();
        AllowSaving(false);
        ShowStatsPanel(false);
        shipDesignSystem.ClearScreen();
        ShowHullPanel();
    }
    #endregion DesignSystemAccess
    #endregion Public
    #endregion Methods

}
