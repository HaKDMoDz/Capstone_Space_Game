using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GalaxyMapData 
{
    public bool[] completeStatus;

    public GalaxyMapData()
    {
        completeStatus = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            completeStatus[i] = false;
        }
    }

    public void Serialize(ref SerializedGalaxyMapData sz_galaxyMapData)
    {
        for (int i = 0; i < 10; i++)
        {
            sz_galaxyMapData.completeStatus[i] = completeStatus[i];
        }
    }
}

[Serializable]
public class SerializedGalaxyMapData
{
    public bool[] completeStatus;

}

