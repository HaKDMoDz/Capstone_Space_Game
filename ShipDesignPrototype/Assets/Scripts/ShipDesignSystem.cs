using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class HullPrefabs
{
    public GameObject frigate;
}
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


public class ShipDesignSystem : MonoBehaviour
{

    public Transform hullPlacementLoc;

    public HullPrefabs hullPrefabs;
    public ComponentPrefabs componentPrefabs;

    

    public void BuildHull(string hullName)
    {
        switch (hullName)
        {
            case "Frigate":
                Instantiate(hullPrefabs.frigate, hullPlacementLoc.position, hullPrefabs.frigate.transform.rotation);
                break;
            default:
                break;
        }
    }

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
