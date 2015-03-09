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

    //private List<ShipBlueprintMetaData> currentFleet = new List<ShipBlueprintMetaData>();
    //public List<ShipBlueprintMetaData> CurrentFleet
    //{
    //    get { return currentFleet; }
    //    set
    //    {
    //        currentFleet = value;
    //        foreach (ShipBlueprintMetaData meta in currentFleet)
    //        {
    //            currentFleetStrength += meta.FleetCost;
    //        }
    //    }
    //}
    private Dictionary<int, ShipBlueprintMetaData> gridIndex_metaData_table = new Dictionary<int, ShipBlueprintMetaData>();

    public Dictionary<int, ShipBlueprintMetaData> GridIndex_metaData_table
    {
        get { return gridIndex_metaData_table; }
        set 
        {
            gridIndex_metaData_table = value;
            foreach (ShipBlueprintMetaData meta in gridIndex_metaData_table.Values)
            {
                currentFleetStrength += meta.FleetCost;
            }
        }
    }

    //private Dictionary<ShipBlueprintMetaData, int> metaData_gridIndex_table = new Dictionary<ShipBlueprintMetaData, int>();
    //public Dictionary<ShipBlueprintMetaData, int> MetaData_gridIndex_table
    //{
    //    get { return metaData_gridIndex_table; }
    //    set
    //    {
    //        metaData_gridIndex_table = value;
    //        foreach (ShipBlueprintMetaData meta in metaData_gridIndex_table.Keys)
    //        {
    //            currentFleetStrength += meta.FleetCost;
    //        }
    //    }
    //}

    #endregion Fields

    #region Methods
    #region Public
    private void AddToFleet(int index, ShipBlueprintMetaData metaData)
    {
        //currentFleet.Add(metaData);
        gridIndex_metaData_table.Add(index, metaData);
        currentFleetStrength += metaData.FleetCost;
        #if FULL_DEBUG
        Debug.Log("Fleet cost: " + metaData.FleetCost + "Current str " + currentFleetStrength);
        #endif
    }
    public bool TryAddToFleet(int index, ShipBlueprintMetaData metaData)
    {
        //a ship already exists in this index
        if (gridIndex_metaData_table.ContainsKey(index))
        {
            ShipBlueprintMetaData currentMeta = gridIndex_metaData_table[index];
            float currentShipCost = currentMeta.FleetCost;
            float newStrength = CurrentFleetStrength - currentShipCost + metaData.FleetCost;
            if (newStrength > maxFleetStrength)
            {
                return false;
            }
            else
            {
                RemoveFromFleet(index);
                AddToFleet(index, metaData);
                return true;
            }
        }
        //index is not occupied
        else
        {
            if (currentFleetStrength + metaData.FleetCost > maxFleetStrength)
            {
                return false;
            }
            else
            {
                AddToFleet(index, metaData);
                return true;
            }
        }
    }
    public bool WouldExceedMaxStr(ShipBlueprintMetaData metaData)
    {
        return (currentFleetStrength + metaData.FleetCost > maxFleetStrength);
    }
    public void RemoveFromFleet(int index)
    {
#if FULL_DEBUG
        if(gridIndex_metaData_table.ContainsKey(index))
        {
            ShipBlueprintMetaData metaData = gridIndex_metaData_table[index];
            currentFleetStrength -= metaData.FleetCost;
            gridIndex_metaData_table.Remove(index);
        }
        else
        {
            Debug.LogError("No blueprint at index " + index);
        }
#else
        ShipBlueprintMetaData metaData = gridIndex_metaData_table[index];
        currentFleetStrength -= metaData.FleetCost;
        gridIndex_metaData_table.Remove(index);
#endif
    }
//    public void RemoveFromFleet(ShipBlueprintMetaData _metaData)
//    {
//#if !NO_DEBUG
//        //if (currentFleet.Any(meta=>meta.Equals(metaData)))
//        if (currentFleet.Any(meta => meta.BlueprintName ==_metaData.BlueprintName))
//        {
//            ShipBlueprintMetaData meta = CurrentFleet.First(metaData => metaData.BlueprintName == _metaData.BlueprintName);
//            currentFleet.Remove(meta);
//            currentFleetStrength -= meta.FleetCost;
//            #if FULL_DEBUG
//            Debug.Log("Removing " + meta.BlueprintName + " Current fleet strength " + currentFleetStrength);
//            #endif
//        }
//        else
//        {
//            Debug.LogError("Ship " + _metaData.BlueprintName +" does not exist in fleet");
//        }
//#else //NO_DEBUG
//        currentFleet.Remove(shipBP);
//#endif
//    }
    public bool CurrentFleetContains(ShipBlueprintMetaData metaData)
    {
        return gridIndex_metaData_table.Any(gridItem => gridItem.Value == metaData);
    }
    #endregion Public

    #endregion Methods
}
