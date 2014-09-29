using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


//[Serializable]
//public class HullPrefabs
//{
//    public GameObject frigate;
//}
//[Serializable]
//public class ComponentPrefabs
//{
//    public GameObject laserCannon;
//    public GameObject missileLauncher;
//    public GameObject shieldGen;
//    public GameObject armour;
//    public GameObject powerPlant;
//    public GameObject repairDrone;

//}


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



    //public HullPrefabs hullPrefabs;
    //public ComponentPrefabs componentPrefabs;

    Dictionary<int, Hull> hullTable;
    Dictionary<int, ShipComponent> compTable;

    public List<Hull> shipHulls;
    public List<ShipComponent> availableComponents;
    public List<ShipBlueprint> availableShips;

    //List<ComponentBlueprint> componentBlueprints
    //List<ComponentUpgrade> availableUpgrades

    ShipBlueprintSaveSystem saveSystem;

    ShipBlueprint currentBlueprint;
    Hull currentHull;
    bool buildingShip = false;
    List<ShipComponent> componentsDisplayed;

    #endregion

    #region Methods

    void Start()
    {
        saveSystem = gameObject.GetSafeComponent<ShipBlueprintSaveSystem>();

        hullTable = hullTableObject.HullTableProp
            .ToDictionary(h => h.ID, h => h.hull);
        compTable = compTableObject.ComponentList
            .ToDictionary(c => c.ID, c => c.component);

        ResetScreen();
        SetupGUI();
    }
    void ResetScreen()
    {
        currentBlueprint = null;
        if (currentHull)
        {
            Destroy(currentHull);
        }
        currentHull = null;
        if (componentsDisplayed == null)
        {
            componentsDisplayed = new List<ShipComponent>();
        }
        else
        {
            for (int i = 0; i < componentsDisplayed.Count; i++)
            {
                Destroy(componentsDisplayed[i]);
            }
            componentsDisplayed.Clear();
        }

    }

    public void SaveBlueprint()
    {
        Debug.Log("SaveBlueprint");
    }
    public void LoadBlueprint()
    {
        Debug.Log("LoadBlueprint");
    }
    public void ClearBlueprint()
    {
        Debug.Log("ClearBlueprint");
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
            Debug.Log("Hull ID: " + id);
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

            if (compTable[i] is Component_Weapon)
            {
                buttonTrans.SetParent(wpnButtonParent);
                offsetCount = ++wpnCount;
            }
            else if (compTable[i] is Component_Defense)
            {
                buttonTrans.SetParent(defButtonParent);
                offsetCount = ++defCount;

            }
            else if (compTable[i] is Component_Power)
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
            Debug.Log("Comp ID: " + id);
            buttonClone.onClick.AddListener(() =>
            {
                BuildComponent(id);
            });
        }



    }
    public void BuildHull(int hullID)
    {
        Debug.Log("ID: " + hullID);
        if (!buildingShip)
        {
            currentHull = Instantiate(hullTable[hullID], hullPlacementLoc.position, hullPlacementLoc.rotation) as Hull;
            currentHull.Init();
            currentBlueprint = new ShipBlueprint(hullTable[hullID]);
            buildingShip = true;
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
            Debug.Log("ID: " + compID);
            Debug.Log("Building " + compTable[compID].name);

            ShipComponent compToBuild = compTable[compID];
            StartCoroutine(StartPlacementSequence(compToBuild));
        }
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
                        ShipComponent otherComp = componentsDisplayed.Find(comp => comp == slot.installedComponent);
                        componentsDisplayed.Remove(otherComp);
                        Destroy(otherComp);
                        slot.installedComponent = null;
                    }
                    componentsDisplayed.Add(Instantiate(component, hit.collider.transform.position, component.transform.rotation) as ShipComponent);
                    currentBlueprint.AddComponent(component, slot);
                    slot.installedComponent = component;
                    runSequence = false;

                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                runSequence = false;
            }
            yield return null;
        }
    }

    //public Vector3 GetBuildPos()
    //{

    //    return Vector3.zero;
    //}

    //public GameObject GetCompPrefab(string compName)
    //{
    //    switch (compName)
    //    {
    //        case "Laser":
    //            return componentPrefabs.laserCannon;
    //        case "Missile":
    //            return componentPrefabs.missileLauncher;
    //        case "Armour":
    //            return componentPrefabs.armour;
    //        case "Shield":
    //            return componentPrefabs.shieldGen;
    //        case "PwrPlant":
    //            return componentPrefabs.powerPlant;
    //        case "Repair":
    //            return componentPrefabs.repairDrone;
    //        default:
    //            return null;
    //    }
    //}

    #endregion

}
