/*
  FleetInterface.cs
  Mission: Invasion
  Created by Rohun Banerji on March 7/2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetInterface : Singleton<FleetInterface>
{
    #region Fields
    //EditorExposed
    [SerializeField]
    private ButtonWithContent buttonPrefab;
    [SerializeField]
    private TextExtended iconCorvettePrefab;
    [SerializeField]
    private TextExtended iconFrigatePrefab;
    [SerializeField]
    private GameObject emptyPrefab;
    [SerializeField]
    private RectTransform gridParent;
    [SerializeField]
    private int gridSize = 21;
    [SerializeField]
    private GameObject fleetPanel;
    [SerializeField]
    private RectTransform savedBPsParent;
    [SerializeField]
    private FillBar fleetStrBar;

    //References
    private FleetManager fleetManager;

    //helper
    private Dictionary<string, GameObject> blueprintName_button_table = new Dictionary<string, GameObject>();
    #endregion Fields

    #region Methods
    //Public
    public void Init()
    {
        fleetManager = FleetManager.Instance;
        SetupGUI();
    }
    public void AddBlueprintButton(string blueprintName)
    {
        ButtonWithContent buttonClone = (ButtonWithContent)Instantiate(buttonPrefab);
        buttonClone.transform.SetParent(savedBPsParent, false);
        buttonClone.SetText(blueprintName);
        buttonClone.AddOnClickListener(() => AddBlueprintToFleet(blueprintName));
        buttonClone.AddOnPointerEnterListener(()=>BPButtonMouseEnter(blueprintName));
        buttonClone.AddOnPointerExitListener(()=>BPButtonMouseExit(blueprintName));
        blueprintName_button_table.Add(blueprintName, buttonClone.gameObject);
    }
    public void RemoveBlueprintButton(string blueprintName)
    {
#if FULL_DEBUG
        if(!blueprintName_button_table.ContainsKey(blueprintName))
        {
            Debug.LogError("Blueprint " + blueprintName + " does not exist in button table");
            return;
        }
        GameObject button;
        if(!blueprintName_button_table.TryGetValue(blueprintName, out button))
        {
            Debug.LogError("No buttons found for blueprint " + blueprintName);
            return;
        }
        else
        {
            Destroy(button);
            blueprintName_button_table.Remove(blueprintName);
        }
#else
        Destroy(blueprintName_button_table[blueprintName]);
        blueprintName_button_table.Remove(blueprintName);
#endif

    }
    public void ClearCurrentFleet()
    {

    }
    public void ShowFleetPanel(bool show)
    {
        fleetPanel.SetActive(show);
    }
    public void OnDeleteBlueprint(string blueprintName)
    {
        if(fleetManager.CurrentFleetContains(ShipDesignSystem.Instance.GetMetaData(blueprintName)))
        {
            ClearCurrentFleet();
        }
        RemoveBlueprintButton(blueprintName);
    }
    public void SaveFleet()
    {
        ShipDesignSystem.Instance.SaveFleet();
        TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.BuildFleet);
    }

    //Private
    private void BPButtonMouseEnter(string blueprintName)
    {
        if (FleetManager.Instance.WouldExceedMaxStr(ShipDesignSystem.Instance.GetMetaData(blueprintName)))
        {
            fleetStrBar.SetFillColour(Color.red);
        }
    }
    private void BPButtonMouseExit(string blueprintName)
    {
        fleetStrBar.SetFillColour(Color.green);
    }
    private void AddBlueprintToFleet(string blueprintName)
    {
        ShipBlueprintMetaData metaData = ShipDesignSystem.Instance.GetMetaData(blueprintName);
        if(fleetManager.TryAddToFleet(metaData))
        {
            AddToCurrentFleet(metaData);
            SetFleetStrBarValue();
        }
        else
        {
            fleetStrBar.SetFillColour(Color.red);
        }
    }
    private void AddToCurrentFleet(ShipBlueprintMetaData metaData)
    {

    }
    private void SetFleetStrBarValue()
    {
        if(!fleetManager)
        {
            Debug.LogError("fleet mg null");
        }
        fleetStrBar.SetValue(((float)fleetManager.CurrentFleetStrength) / (float)fleetManager.MaxFleetStrength);
    }
    private void SetupGUI()
    {
        //setup empty grid
        for (int i = 0; i < gridSize; i++)
        {
            GameObject emptyClone = (GameObject)Instantiate(emptyPrefab);
            emptyClone.transform.SetParent(gridParent, false);
        }
        //fleet str bar
        SetFleetStrBarValue();
        //blueprint buttons
        foreach (string blueprintName in ShipDesignSystem.Instance.GetSaveFileList())
        {
            AddBlueprintButton(blueprintName);
        }
    }

    
    #endregion Methods
}
