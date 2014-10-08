using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;



public class ShipDesignSystem : SingletonComponent<ShipDesignSystem>
{

    #region Fields
    //assigned from Editor
    [SerializeField]
    Transform hullPlacementLoc;
    [SerializeField]
    HullTable hullTableObject;
    [SerializeField]
    ComponentTable compTableObject;

    //GUI stuff
    [SerializeField]
    Canvas canvas;

    [SerializeField]
    RectTransform wpnButtonParent;
    [SerializeField]
    RectTransform defButtonParent;
    [SerializeField]
    RectTransform pwrButtonParent;
    [SerializeField]
    RectTransform supButtonParent;

    [SerializeField]
    RectTransform hullButtonParent;
    [SerializeField]
    RectTransform defaultHullButtonPos;
    [SerializeField]
    RectTransform defaultCompButtonPos;
    [SerializeField]
    Button buttonPrefab;
    [SerializeField]
    float buttonYOffset = 20f;
    [SerializeField]
    DialogueBox dialogueBox;




    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;

    public List<Hull> shipHulls;
    public List<ShipComponent> availableComponents;
    public List<ShipBlueprint> availableShips;

    //List<ComponentBlueprint> componentBlueprints
    //List<ComponentUpgrade> availableUpgrades

    ShipBlueprint currentBlueprint;
    Hull currentHull;
    bool buildingShip = false;
    List<ShipComponent> componentsDisplayed;

    #endregion

    #region Methods

    #region Debug
    void OnGUI()
    {
        if (currentBlueprint != null)
        {
            GUILayout.BeginVertical();
            foreach (var item in currentBlueprint.ComponentTable)
            {
                GUILayout.Label(item.Key.index + ": " + item.Value.componentName);
            }
            GUILayout.EndVertical();
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            SaveBlueprint("");
        }
        if(Input.GetKeyDown(KeyCode.F9))
        {
            LoadBlueprint();
        }
        if(Input.GetKeyDown(KeyCode.F12))
        {
            ClearBlueprint();
        }
    }
    #endregion

    void Start()
    {

        hullTable = hullTableObject.HullTableProp
            .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObject.ComponentList
            .ToDictionary(c => c.ID, c => c.component);
        //inputField.validation = InputField.Validation.Alphanumeric;
        //inputField.onSubmit.AddListener(
        //    (value)=> { Debug.Log("submit: " + value); }
        //    );

        
        //configure dialogue box
        dialogueBox.inputField.validation = InputField.Validation.Alphanumeric;
        dialogueBox.inputField.onSubmit.AddListener(
            (value) => { SaveBlueprint(value); });
        dialogueBox.confirmButton.onClick.AddListener(
            () => { SaveBlueprint(dialogueBox.inputField.value); });
        dialogueBox.cancelButton.onClick.AddListener(
            () => { ShowSaveDialogueBox(false); });
        ShowSaveDialogueBox(false);


        buttonYOffset = Screen.height * .055f;

        ResetScreen();
        SetupGUI();
    }
    void ResetScreen()
    {
        buildingShip = false;
        currentBlueprint = null;
        if (currentHull)
        {
            //Debug.Log("Destroying hull");
            Destroy(currentHull.gameObject);
        }
        currentHull = null;
        if (componentsDisplayed == null)
        {
            componentsDisplayed = new List<ShipComponent>();
            //Debug.Log(componentsDisplayed.Count);
        }
        else
        {
            for (int i = 0; i < componentsDisplayed.Count; i++)
            {
                Destroy(componentsDisplayed[i].gameObject);
            }
            componentsDisplayed.Clear();
        }

    }
    public void ShowSaveDialogueBox(bool showing)
    {
        if(!showing)
        {
            dialogueBox.gameObject.SetActive(false);
        }
            //showing dialogue box
        else if (currentBlueprint != null)
        {
            dialogueBox.gameObject.SetActive(true);

            //will not work after update
            EventSystemManager.currentSystem.SetSelectedGameObject(dialogueBox.inputField.gameObject, null);
            dialogueBox.inputField.OnPointerClick(null);

            //may need to change above code to after update:
            /*
             * EventSystem.current.SetSelectedGameObject(input.gameObject, null);
                input.OnPointerClick(null);*/

        }
        else
        {
            Debug.Log("no ship being designed");
        }
    }
    void SaveBlueprint(string fileName)
    {
        //Debug.Log("SaveBlueprint");
            ShipBlueprintSaveSystem.Instance.Save(currentBlueprint, fileName);
            ShowSaveDialogueBox(false);
    }
    public void LoadBlueprint()
    {
        Debug.Log("LoadBlueprint");
        ResetScreen();
        if( ShipBlueprintSaveSystem.Instance.Load(out currentBlueprint))
        {
            AddHullToDisplay(currentBlueprint.Hull);
            foreach (var item in currentBlueprint.ComponentTable)
            {
                AddCompToDisplay(item.Key, item.Value);
            }
            buildingShip = true;
        }
        else
        {
            Debug.Log("No saved ship blueprints found");
        }
    }
    public void ClearBlueprint()
    {
        //Debug.Log("ClearBlueprint");
        ResetScreen();
    }

    

    void SetupGUI()
    {
        Button buttonClone;
        RectTransform buttonTrans;

        //Hulls GUI
        for (int i = 0; i < hullTable.Count; i++)
        {
            buttonClone = Instantiate(buttonPrefab) as Button;
            buttonTrans = buttonClone.GetComponent<RectTransform>();
            buttonTrans.SetParent(hullButtonParent);
            buttonTrans.CopyTransform(defaultHullButtonPos);
            buttonTrans.SetPositionY(buttonTrans.position.y - buttonYOffset * i);
            buttonClone.GetComponentInChildren<Text>().text = hullTable.ElementAt(i).Value.name;
            int id = hullTable.ElementAt(i).Key;
            //Debug.Log("Hull ID: " + id);
            buttonClone.onClick.AddListener(() =>
                {
                    //BuildHull((int)hullTable.ElementAt(i).Key);
                    BuildHull(id);
                });
        }

        //Components GUI

        int wpnCount = 0, defCount = 0, pwrCount = 0, supCount = 0;
        int offsetCount;
        for (int i = 0; i < compTable.Count; i++)
        {
            buttonClone = Instantiate(buttonPrefab) as Button;
            buttonTrans = buttonClone.GetComponent<RectTransform>();
            buttonClone.gameObject.GetComponentInChildren<Text>().text = compTable.ElementAt(i).Value.componentName;

            if (compTable[i].CompType==ShipComponent.ComponentType.Weapon)
            {
                buttonTrans.SetParent(wpnButtonParent);
                offsetCount = ++wpnCount;
            }
            else if (compTable[i].CompType == ShipComponent.ComponentType.Defense)
            {
                buttonTrans.SetParent(defButtonParent);
                offsetCount = ++defCount;

            }
            else if (compTable[i].CompType == ShipComponent.ComponentType.Power)
            {
                buttonTrans.SetParent(pwrButtonParent);
                offsetCount = ++pwrCount;
            }
            else
            {
                buttonTrans.SetParent(supButtonParent);
                offsetCount = ++supCount;
            }
            //buttonTrans.SetParent(wpnButtonParent);
            buttonTrans.CopyTransform(defaultCompButtonPos);
            buttonTrans.SetPositionY(buttonTrans.position.y - buttonYOffset * (offsetCount - 1));

            //id = compTable.ElementAt(i).Key;
            int id = compTable.ElementAt(i).Key;
            //Debug.Log("Comp ID: " + id);
            buttonClone.onClick.AddListener(() =>
            {
                BuildComponent(id);
            });
        }



    }
    public void BuildHull(int hullID)
    {
        //Debug.Log("ID: " + hullID);
        if (!buildingShip)
        {
            
            AddHullToDisplay(hullTable[hullID]);
            currentHull.Init();
            currentBlueprint = new ShipBlueprint(hullTable[hullID]);
            buildingShip = true;
            //currentBlueprint.OutputContents();
        }
        else
        {
            Debug.Log("Already building a ship");
        }
    }

    public void BuildComponent(int compID)
    {
        if (buildingShip)
        {
            //Debug.Log("ID: " + compID);
            //Debug.Log("Building " + compTable[compID].name);

            ShipComponent compToBuild = compTable[compID];
            StartCoroutine(StartPlacementSequence(compToBuild));
            
        }
    }

    void AddHullToDisplay(Hull hull)
    {
        currentHull = Instantiate(hull, hullPlacementLoc.position, hullPlacementLoc.rotation) as Hull;
    }

    ShipComponent AddCompToDisplay(ShipComponent component, Vector3 pos, Quaternion rot)
    {                   //clone
        ShipComponent builtComp = Instantiate(component, pos, rot) as ShipComponent;
        componentsDisplayed.Add(builtComp);//clone
        return builtComp;//clone
    }                                                       //original
    void AddCompToDisplay(ComponentSlot slot,  ShipComponent component)
    {                   //clone
        ShipComponent builtComp = Instantiate(component, slot.transform.position, component.transform.rotation) as ShipComponent;
        componentsDisplayed.Add(builtComp);//clone
    }

    IEnumerator StartPlacementSequence(ShipComponent component)
    {
        bool runSequence = true;
        Ray ray;
        RaycastHit hit;

        while (runSequence)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000f, 1 << GlobalTagsAndLayers.Instance.layers.componentTileLayer))
                {
                    ComponentSlot slot = hit.transform.GetComponent<ComponentSlot>();
                    if (slot.installedComponent)
                    {
                        ShipComponent otherComp = componentsDisplayed.Find(comp => comp.componentName == slot.installedComponent.componentName);
                        Debug.Log(slot.installedComponent);
                       
                        componentsDisplayed.Remove(otherComp);
                        Destroy(otherComp.gameObject);
                        //slot.installedComponent = null;
                        currentBlueprint.RemoveComponent(slot);

                    }
                    //ShipComponent builtComp = Instantiate(component, hit.collider.transform.position, component.transform.rotation) as ShipComponent;

                                //clone
                    AddCompToDisplay(component, hit.collider.transform.position, component.transform.rotation);

                    //componentsDisplayed.Add(builtComp);
                    //Debug.Log("Components Displays count: " + componentsDisplayed.Count);
                    currentBlueprint.AddComponent(component, slot);
                    
                    //slot.installedComponent = builtComp;
                    runSequence = false;

                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                runSequence = false;
            }
            yield return null;
        }
        //currentBlueprint.OutputContents();
    }


    #endregion

}
