using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ComponentPrefabs
{
    public GameObject laserCannon;
    public GameObject missileLauncher;
    public GameObject shieldGen;
    public GameObject powerPlant;

}


public class ShipDesignSystem : MonoBehaviour
{

    public ComponentPrefabs componentPrefabs;

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
            default:
                return null;
        }
    }



}
