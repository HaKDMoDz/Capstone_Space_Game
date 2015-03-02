using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : Singleton<FleetManager>
{

    #region Fields
    //EditorExposed
    [SerializeField]
    private int maxFleetStrength;
    private int currentFleetStrength;
    public int CurrentFleetStrength
    {
        get { return currentFleetStrength; }
    }

    private List<ShipBlueprintMetaData> currentFleet = new List<ShipBlueprintMetaData>();
    public List<ShipBlueprintMetaData> CurrentFleet
    {
        get { return currentFleet; }
        set
        {
            currentFleet = value;
            //foreach (string blueprintName in currentFleet)
            //{
            //    ShipDesignInterface.Instance.AddCurrentFleetButton(blueprintName);
            //}
        }
    }

    #endregion Fields

    #region Methods
    #region Public
    private void AddToFleet(ShipBlueprintMetaData metaData)
    {
        currentFleet.Add(metaData);
        currentFleetStrength += metaData.FleetCost;
#if FULL_DEBUG
        Debug.Log("Fleet cost: " + metaData.FleetCost + "Current str " + currentFleetStrength);
#endif
    }
    public bool TryAddToFleet(ShipBlueprintMetaData metaData)
    {
        if(currentFleetStrength + metaData.FleetCost > maxFleetStrength)
        {
            return false;
        }
        else
        {
            AddToFleet(metaData);
            return true;
        }
    }
    public void RemoveFromFleet(ShipBlueprintMetaData metaData)
    {
#if !NO_DEBUG
        if (currentFleet.Contains(metaData))
        {
            currentFleet.Remove(metaData);
            currentFleetStrength -= metaData.FleetCost;
            #if FULL_DEBUG
            Debug.Log("Current fleet strength " + currentFleetStrength);
            #endif
        }
        else
        {
            Debug.LogError("Ship does not exist in fleet");
        }
#else //NO_DEBUG
        currentFleet.Remove(shipBP);
#endif
    }
    public bool CurrentFleetContains(ShipBlueprintMetaData metaData)
    {
        return currentFleet.Contains(metaData);
    }
    #endregion Public

    #endregion Methods
}
