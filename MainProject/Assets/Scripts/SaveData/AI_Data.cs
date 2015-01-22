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

    SerializedAI_Data Serialized()
    {
        return new SerializedAI_Data();
    }

    public void Serialize(ref SerializedAI_Data sz_AIFleetData)
    {
        Debug.Log(sz_AIFleetData.currentFleet_BlueprintNames);
        Debug.Log(currentFleet_BlueprintNames);
        sz_AIFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;
    }

}

[Serializable]
public class SerializedAI_Data
{
    public List<string> currentFleet_BlueprintNames;

    public SerializedAI_Data()
    {
        currentFleet_BlueprintNames = new List<string>();
    }
    public void DeSerialize(ref PlayerFleetData AIFleetData)
    {
        AIFleetData.currentFleet_BlueprintNames = currentFleet_BlueprintNames;
    }
}
