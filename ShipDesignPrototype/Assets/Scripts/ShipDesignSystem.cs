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
[Serializable]
public class ComponentPrefabs
{
    public GameObject laserCannon;
    public GameObject missileLauncher;
    public GameObject shieldGen;
    public GameObject armour;
    public GameObject powerPlant;
    public GameObject repairDrone;

}


public class ShipDesignSystem : SingletonComponent<ShipDesignSystem>
{
    //assigned from Editor
    [SerializeField]
    Transform hullPlacementLoc;
    [SerializeField]
    HullTable hullTableObject;

    //GUI stuff
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    RectTransform compButtonParent;
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
    public ComponentPrefabs componentPrefabs;

    Dictionary<int, GameObject> hullTable;

    public List<Hull> shipHulls;
    public List<ShipComponent> availableComponents;
    public List<ShipBlueprint> availableShips;

    //List<ComponentBlueprint> componentBlueprints
    //List<ComponentUpgrade> availableUpgrades

    void Start()
    {
        hullTable = hullTableObject.HullTable1
            .ToDictionary(hull => hull.ID, hull => hull.hullPrefab);

        SetupGUI();
    }

    void SetupGUI()
    {
        Button buttonClone;
        RectTransform buttonTrans;
        for (int i = 0; i < hullTable.Count; i++)
        {
            buttonClone= Instantiate(buttonPrefab) as Button;
            buttonTrans= buttonClone.GetComponent<RectTransform>();
            buttonTrans.SetParent(hullButtonParent);
            buttonTrans.CopyTransform(defaultHullButtonPos);
            buttonTrans.SetPositionY(buttonTrans.position.y - buttonYOffset * i);
            buttonClone.GetComponentInChildren<Text>().text = hullTable.ElementAt(i).Value.name;
            int num = hullTable.ElementAt(i).Key;
            buttonClone.onClick.AddListener(() =>
                {
                   // BuildHull((int)hullTable.ElementAt(i).Key);
                    BuildHull(num);
                });
        }
    }
    public void BuildHull(int hullID)
    {
        Instantiate(hullTable[hullID], hullPlacementLoc.position, hullPlacementLoc.rotation);
    }
    //public void BuildHull(string hullName)
    //{
    //    switch (hullName)
    //    {
    //        case "Frigate":
    //            Instantiate(hullPrefabs.frigate, hullPlacementLoc.position, hullPrefabs.frigate.transform.rotation);
    //            break;
    //        case "New":
    //            Debug.Log("Create new hull");
    //            break;
    //        default:
    //            break;
    //    }
    //}

    public void BuildComponent(string compName)
    {
        Debug.Log("Building " + compName);

        GameObject compToBuild = GetCompPrefab(compName);

        StartCoroutine(StartPlacementSequence(compToBuild));
        //Vector3 buildPos = GetBuildPos();
        //Instantiate(compToBuild, buildPos, compToBuild.transform.rotation);

    }


    IEnumerator StartPlacementSequence(GameObject compPrefab)
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
                    Instantiate(compPrefab, hit.collider.transform.position, compPrefab.transform.rotation);
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

    public Vector3 GetBuildPos()
    {

        return Vector3.zero;
    }

    public GameObject GetCompPrefab(string compName)
    {
        switch (compName)
        {
            case "Laser":
                return componentPrefabs.laserCannon;
            case "Missile":
                return componentPrefabs.missileLauncher;
            case "Armour":
                return componentPrefabs.armour;
            case "Shield":
                return componentPrefabs.shieldGen;
            case "PwrPlant":
                return componentPrefabs.powerPlant;
            case "Repair":
                return componentPrefabs.repairDrone;
            default:
                return null;
        }
    }



}
