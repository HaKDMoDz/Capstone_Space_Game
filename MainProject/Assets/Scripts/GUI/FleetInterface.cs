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
    //Cursors
    [SerializeField]
    private Texture2D defaultCursor;
    [SerializeField]
    private Texture2D shipCursor;
    //References
    private FleetManager fleetManager;

    //helper
    private Dictionary<string, GameObject> blueprintName_button_table = new Dictionary<string, GameObject>();
    private List<FleetGridItem> gridItemList = new List<FleetGridItem>();
    private bool fleetPositioning = false;
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
        fleetPositioning = false;
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
    private void BPButtonClick(string blueprintName)
    {
        //AddBlueprintToFleet(blueprintName);
//        Canvas canvas = fleetPanel.GetComponentInParent<Canvas>();
//#if FULL_DEBUG
//        if(!canvas)
//        {
//            Debug.LogError("No canvas found");
//        }
//#endif
        //GameObject iconObj = new GameObject("shipIcon");
        //iconObj.transform.SetParent(canvas.transform, false);
        //iconObj.transform.SetAsLastSibling();
        //Image image = iconObj.AddComponent<Image>();
        //image.sprite = shipIcon.GetComponentInChildren<Image>().sprite;
        //TextExtended shipIconPrefab = ShipDesignSystem.Instance.GetHull(blueprintName).HullIcon;
        //TextExtended shipIcon = Instantiate(shipIconPrefab) as TextExtended;
        //RectTransform shipIconTrans = (RectTransform)shipIcon.transform;
        //shipIconTrans.SetParent(canvas.transform, false);
        //shipIconTrans.SetAsLastSibling();
        //shipIcon.ShowText(false);
        //shipIconTrans.sizeDelta *= 0.5f;
        //shipIcon.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
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
        if (AddBlueprintToFleet(selectedBlueprintName))
        {
            TextExtended shipIconPrefab = ShipDesignSystem.Instance.GetHull(selectedBlueprintName).HullIcon;
            TextExtended shipIcon = Instantiate(shipIconPrefab) as TextExtended;
            shipIcon.SetText(selectedBlueprintName);
            shipIcon.transform.SetParent(gridParent, false);
            shipIcon.transform.SetSiblingIndex(gridItem.Index + 1);
            FleetGridItem shipGridItem = shipIcon.GetComponent<FleetGridItem>();
            shipGridItem.Index = gridItem.Index;
            gridItemList.Add(shipGridItem);
            gridItemList.Remove(gridItem);
            Destroy(gridItem.gameObject);
            fleetPositioning = false;
        }
    }
    private void SubscribeToGridEvents(bool subscribe)
    {
        foreach (FleetGridItem gridItem in gridItemList)
        {
            if(subscribe)
            {
                gridItem.OnGridPointerClick += OnGridPointerClick;
            }
            else
            {
                gridItem.OnGridPointerClick -= OnGridPointerClick;
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
    private bool AddBlueprintToFleet(string blueprintName)
    {
        ShipBlueprintMetaData metaData = ShipDesignSystem.Instance.GetMetaData(blueprintName);
        if(fleetManager.TryAddToFleet(metaData))
        {
            AddToCurrentFleet(metaData);
            SetFleetStrBarValue();
            return true;
        }
        else
        {
            fleetStrBar.SetFillColour(Color.red);
            return false;
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
            FleetGridItem emptyClone = (FleetGridItem)Instantiate(emptyPrefab);
            emptyClone.transform.SetParent(gridParent, false);
            emptyClone.Index = i;
            gridItemList.Add(emptyClone);
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
