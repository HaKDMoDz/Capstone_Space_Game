using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerFleetData
{
    public List<ShipBlueprintMetaData> currentFleet_meta_list;

    public PlayerFleetData()
    {
        currentFleet_meta_list = new List<ShipBlueprintMetaData>();
    }

    public void Serialize(ref SerializedPlayerFleetData sz_playerFleetData)
    {
        sz_playerFleetData.currentFleet_meta_list = currentFleet_meta_list;    
    }
}

[Serializable]
public class SerializedPlayerFleetData
{
    public List<ShipBlueprintMetaData> currentFleet_meta_list;

    public SerializedPlayerFleetData()
    {
        currentFleet_meta_list = new List<ShipBlueprintMetaData>();
    }
    public void DeSerialize(ref PlayerFleetData playerFleetData)
    {
        playerFleetData.currentFleet_meta_list = currentFleet_meta_list;
    }
}