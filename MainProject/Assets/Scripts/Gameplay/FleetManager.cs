/*
  FleetManager.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 8/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FleetManager : Singleton<FleetManager>
{

    #region Fields
    //EditorExposed
    [SerializeField]
    private int maxFleetStrength;
    public int MaxFleetStrength
    {
        get { return maxFleetStrength; }
    }
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
            foreach (ShipBlueprintMetaData meta in currentFleet)
            {
                currentFleetStrength += meta.FleetCost;
            }
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
    public bool WouldExceedMaxStr(ShipBlueprintMetaData metaData)
    {
        return (currentFleetStrength + metaData.FleetCost > maxFleetStrength);
    }
    public void RemoveFromFleet(ShipBlueprintMetaData _metaData)
    {
#if !NO_DEBUG
        //if (currentFleet.Any(meta=>meta.Equals(metaData)))
        if (currentFleet.Any(meta => meta.BlueprintName ==_metaData.BlueprintName))
        {
            ShipBlueprintMetaData meta = CurrentFleet.First(metaData => metaData.BlueprintName == _metaData.BlueprintName);
            currentFleet.Remove(meta);
            currentFleetStrength -= meta.FleetCost;
            #if FULL_DEBUG
            Debug.Log("Removing " + meta.BlueprintName + " Current fleet strength " + currentFleetStrength);
            #endif
        }
        else
        {
            Debug.LogError("Ship " + _metaData.BlueprintName +" does not exist in fleet");
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
