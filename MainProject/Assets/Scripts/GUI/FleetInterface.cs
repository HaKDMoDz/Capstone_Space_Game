/*
  FleetInterface.cs
  Mission: Invasion
  Created by Rohun Banerji on March 7/2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private FleetGridItem emptyPrefab;
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
    [SerializeField]
    private float blinkDuration = 1.0f;
    [SerializeField]
    private float blinkFrequency = 0.2f;
    //Cursors
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Texture2D shipCursor;
    [SerializeField]
    private Texture2D deleteCursor;
    //References
    private FleetManager fleetManager;

    //helper
    private Dictionary<string, GameObject> blueprintName_button_table = new Dictionary<string, GameObject>();
    private List<FleetGridItem> gridItemList = new List<FleetGridItem>();
    private bool fleetPositioning = false;
    private bool fleetDeletion = false;
    private string selectedBlueprintName;
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
        buttonClone.AddOnClickListener(() => BPButtonClick(blueprintName));
        //buttonClone.AddOnPointerEnterListener(()=>BPButtonMouseEnter(blueprintName));
        //buttonClone.AddOnPointerExitListener(()=>BPButtonMouseExit(blueprintName));
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
        fleetPositioning = false;
        fleetPanel.SetActive(show);
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
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
    public void DeleteFromFleet()
    {
        StopCoroutine("FleetDeletion");
        SubscribeToGridShipEvents(false);
        StartCoroutine("FleetDeletion");
    }
    //Private
    private IEnumerator FleetDeletion()
    {
        Cursor.SetCursor(deleteCursor, Vector2.zero, CursorMode.Auto);
        fleetDeletion = true;
        SubscribeToGridShipEvents(true);
        while (fleetDeletion)
        {
            //wait for DeleteShipItem
            yield return null;
        }
        SubscribeToGridShipEvents(false);
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    private void DeleteShipItem(FleetGridItem gridItem)
    {
        fleetManager.RemoveFromFleet(gridItem.Index);
        RefreshFleetStrBarValue();
        FleetGridItem emptyClone = (FleetGridItem)Instantiate(emptyPrefab);
        AddToGridAtIndex(emptyClone, gridItem.Index);
        fleetDeletion = false;
    }
    
    private void BPButtonClick(string blueprintName)
    {
        selectedBlueprintName = blueprintName;
        StopCoroutine("FleetPositioning");
        SubscribeToGridEvents(false);
        StartCoroutine("FleetPositioning");
    }
    private IEnumerator FleetPositioning()
    {
        Cursor.SetCursor(shipCursor, Vector2.zero, CursorMode.Auto);
        fleetPositioning = true;
        SubscribeToGridEvents(true);
        while (fleetPositioning)
        {
            //waiting for OnGridPointerClick 
            yield return null;
        }
        SubscribeToGridEvents(false);
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    private void OnGridPointerClick(FleetGridItem gridItem)
    {
        Debug.Log("Pointer Click: Grid Item " + gridItem.Index);
        if (AddBlueprintToFleet(selectedBlueprintName, gridItem.Index))
        {
            TextExtended shipIconPrefab = ShipDesignSystem.Instance.GetHull(selectedBlueprintName).HullIcon;
            TextExtended shipIcon = Instantiate(shipIconPrefab) as TextExtended;
            shipIcon.SetText(selectedBlueprintName);
            //shipIcon.transform.SetParent(gridParent, false);
            //shipIcon.transform.SetSiblingIndex(gridItem.Index + 1);
            FleetGridItem shipGridItem = shipIcon.GetComponent<FleetGridItem>();
            AddToGridAtIndex(shipGridItem, gridItem.Index);
            //shipGridItem.Index = gridItem.Index;
            shipGridItem.IsEmpty = false;
            //gridItemList.Add(shipGridItem);
            //gridItemList.Remove(gridItem);
            //Destroy(gridItem.gameObject);
            fleetPositioning = false;
        }
    }
    private void OnGridPointerEnter(FleetGridItem gridItem)
    {
        gridItem.Image.color = Color.green.WithAplha(1.0f);
    }
    private void OnGridPointerExit(FleetGridItem gridItem)
    {
        gridItem.Image.color = Color.white.WithAplha(90.0f / 255.0f);
    }
    private void AddToGridAtIndex(FleetGridItem gridItem, int index)
    {
        FleetGridItem oldItem = gridItemList.Find(item => item.Index == index);
        gridItem.transform.SetParent(gridParent, false);
        gridItem.transform.SetSiblingIndex(index + 1);
        gridItem.Index = index;
        gridItemList.Add(gridItem);
        gridItemList.Remove(oldItem);
        Destroy(oldItem.gameObject);
        
    }
    private void SubscribeToGridEvents(bool subscribe)
    {
        foreach (FleetGridItem gridItem in gridItemList)
        {
            if(subscribe)
            {
                gridItem.OnGridPointerClick += OnGridPointerClick;
                gridItem.OnGridPointerEnter += OnGridPointerEnter;
                gridItem.OnGridPointerExit += OnGridPointerExit;
            }
            else
            {
                gridItem.OnGridPointerClick -= OnGridPointerClick;
                gridItem.OnGridPointerEnter -= OnGridPointerEnter;
                gridItem.OnGridPointerExit -= OnGridPointerExit;
            }
        }
    }
    private void SubscribeToGridShipEvents(bool subscribe)
    {
        foreach (FleetGridItem gridItem in gridItemList.Where(item=>!item.IsEmpty))
        {
            if(subscribe)
            {
                gridItem.OnGridPointerClick += DeleteShipItem;
                gridItem.OnGridPointerEnter += OnGridPointerEnter;
                gridItem.OnGridPointerExit += OnGridPointerExit;
            }
            else
            {
                gridItem.OnGridPointerClick -= DeleteShipItem;
                gridItem.OnGridPointerEnter -= OnGridPointerEnter;
                gridItem.OnGridPointerExit -= OnGridPointerExit;
            }
        }
    }
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
    private bool AddBlueprintToFleet(string blueprintName, int index)
    {
        ShipBlueprintMetaData metaData = ShipDesignSystem.Instance.GetMetaData(blueprintName);
        if (fleetManager.TryAddToFleet(index, metaData))
        {
            //AddToCurrentFleet(metaData);
            RefreshFleetStrBarValue();
            return true;
        }
        else
        {
            StartCoroutine(InvalidFleetStrength());
            return false;
        }
    }
    private IEnumerator InvalidFleetStrength()
    {
        //fleetStrBar.SetFillColour(Color.red);
        bool barIsOn = true;
        float currentTime = 0.0f;
        while(currentTime <= blinkDuration)
        {
            if (barIsOn)
            {
                fleetStrBar.SetFillColour(Color.red);
            }
            else
            {
                fleetStrBar.SetFillColour(Color.red.WithAplha(0.0f));
            }
            yield return new WaitForSeconds(blinkFrequency);
            barIsOn = !barIsOn;
            currentTime += blinkFrequency;
        }
        fleetStrBar.SetFillColour(Color.green);
    }
    private void RefreshFleetStrBarValue()
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
            FleetGridItem emptyClone = (FleetGridItem)Instantiate(emptyPrefab);
            emptyClone.transform.SetParent(gridParent, false);
            emptyClone.Index = i;
            gridItemList.Add(emptyClone);
        }
        //fleet str bar
        RefreshFleetStrBarValue();
        //blueprint buttons
        foreach (string blueprintName in ShipDesignSystem.Instance.GetSaveFileList())
        {
            AddBlueprintButton(blueprintName);
        }
    }

    
    #endregion Methods
}
