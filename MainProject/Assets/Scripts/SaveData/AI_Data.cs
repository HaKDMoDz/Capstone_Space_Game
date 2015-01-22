using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AI_Data
{
    public List<string> currentFleet_BlueprintNames;

    public AI_Data()
    {
        currentFleet_BlueprintNames = new List<string>();
    }

    public void Serialize(ref SerializedAI_Data sz_AIFleetData)
    {
        Debug.Log(currentFleet_BlueprintNames);
        sz_AIFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;
    }

}

[Serializable]
public class SerializedAI_Data
{
    public List<string> currentFleet_BlueprintNames;

    public void DeSerialize(ref PlayerFleetData AIFleetData)
    {
        AIFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;
    }
}
