using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    public InputDialogueBox saveDialogueBox;
    public GameObject loadPanel;
    public RectTransform loadButtonParent;
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
    #region GUIBuilders
    private void SetupGUI()
    {
        //Setup Hull Buttons
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
        //Setup Component Buttons
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
        //Setup Load Panel
        foreach (string blueprintName in ShipDesignSystem.Instance.GetSaveFileList())
        {
            AddLoadButton(blueprintName);
        }

        //Setup Save dialogue box
        guiFields.saveDialogueBox.inputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
        guiFields.saveDialogueBox.inputField.onEndEdit.AddListener((value) =>
            {
                SaveInputFieldSubmit(guiFields.saveDialogueBox.inputText.text);
            });
        guiFields.saveDialogueBox.submitButton.onClick.AddListener(() =>
            {
                SaveBlueprint(guiFields.saveDialogueBox.inputText.text);
            });
        guiFields.saveDialogueBox.cancelButton.onClick.AddListener(() =>
            {
                ShowSaveDialogueBox(false);
            });

        
    }
    private void AddLoadButton(string fileName)
    {
        ButtonWithContent buttonClone = Instantiate(guiFields.buttonPrefab) as ButtonWithContent;
        buttonClone.transform.SetParent(guiFields.loadButtonParent, false);
        buttonClone.buttonText.text = fileName;
        buttonClone.button.onClick.AddListener(() =>
            {
                LoadBlueprint(fileName);
            });
    }
    #endregion GUIBuilders



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

    private void ClearGUI()
    {
        ShowSaveDialogueBox(false);
        ShowLoadPanel(false);
    }
    #endregion Private
    
    #region Public
    #region GUIAccess
    public void SelectComponentToBuild(int componentID)
    {
        #if FULL_DEBUG
        Debug.Log("Selected component: " + ComponentTable.GetComp(componentID).componentName);
        #endif
        StartCoroutine(StartPlacementSequence(ComponentTable.GetComp(componentID)));
    }
    public void SaveInputFieldSubmit(string inputText)
    {
        if(Input.GetButtonDown("Submit"))
        {
            SaveBlueprint(inputText);
        }
        if(Input.GetButtonDown("Cancel"))
        {
            ShowSaveDialogueBox(false);
        }
    }
    public void ShowSaveDialogueBox(bool show)
    {
        if (!show)
        {
            guiFields.saveDialogueBox.gameObject.SetActive(false);
        }
        //showing dialogue box
        else if(ShipDesignSystem.Instance.buildingShip)
        {
            guiFields.saveDialogueBox.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(guiFields.saveDialogueBox.inputField.gameObject, null);
            guiFields.saveDialogueBox.inputField.ActivateInputField();
        }
        #if FULL_DEBUG
        else
        {
            Debug.Log("No Ship being built");
        }
        #endif

    }
    public void ShowLoadPanel(bool show)
    {
        guiFields.loadPanel.SetActive(show);
    }
    #endregion GUIAccess

    #region DesignSystemAccess
    public void BuildHull(int hull_ID)
    {
        ShipDesignSystem.Instance.BuildHull(hull_ID);
    }
    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {
        ShipDesignSystem.Instance.BuildComponent(slot, component);
    }
    //GUI should not access directly
    private void SaveBlueprint(string fileName)
    {
        ClearGUI();
        ShipDesignSystem.Instance.SaveBlueprint(fileName);
        AddLoadButton(fileName);
    }
    //GUI should not access directly
    private void LoadBlueprint(string fileName)
    {
        ClearGUI();
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
        ClearGUI();
        ShipDesignSystem.Instance.ClearScreen();
    }
    #endregion DesignSystemAccess
    #endregion Public
    #endregion Methods

}
