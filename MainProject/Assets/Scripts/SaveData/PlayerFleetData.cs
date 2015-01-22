using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerFleetData
{
    public List<string> currentFleet_BlueprintNames;

    public PlayerFleetData()
    {
        currentFleet_BlueprintNames = new List<string>();
    }

    public void Serialize(ref SerializedPlayerFleetData sz_playerFleetData)
    {
        sz_playerFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;    
    }
}

[Serializable]
public class SerializedPlayerFleetData
{
    public List<string> currentFleet_BlueprintNames;

    public SerializedPlayerFleetData()
    {
        currentFleet_BlueprintNames = new List<string>();
    }
    public void DeSerialize(ref PlayerFleetData playerFleetData)
    {
        playerFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;
    }
}