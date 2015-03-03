using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GalaxyMapData 
{
    public bool[] completeStatus;
    public int currentMissionID;
    public Vector3 position;

    public GalaxyMapData()
    {
        completeStatus = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            completeStatus[i] = false;
        }
        currentMissionID = 0;
        position = Vector3.zero;
    }

    public void Serialize(ref SerializedGalaxyMapData sz_galaxyMapData)
    {
        for (int i = 0; i < 10; i++)
        {
            sz_galaxyMapData.sz_completeStatus[i] = completeStatus[i];
        }
        sz_galaxyMapData.sz_currentMissionID = currentMissionID;
        sz_galaxyMapData.sz_positionX = position.x;
        sz_galaxyMapData.sz_positionY = position.y;
        sz_galaxyMapData.sz_positionZ = position.z;
    }
}

[Serializable]
public class SerializedGalaxyMapData
{
    public bool[] sz_completeStatus;
    public int sz_currentMissionID;
    public float sz_positionX, sz_positionY, sz_positionZ;

    public SerializedGalaxyMapData()
    {
        sz_completeStatus = new bool[10];
        for (int i = 0; i < 10; i++)
        {
            sz_completeStatus[i] = false;
        }
        sz_currentMissionID = 0;
        sz_positionX = 0.0f;
        sz_positionY = 0.0f;
        sz_positionZ = 0.0f;
    }

    public void DeSerialize(ref GalaxyMapData _galaxyMapData)
    {
        for (int i = 0; i < 10; i++)
        {
            _galaxyMapData.completeStatus[i] = sz_completeStatus[i];
        }
        _galaxyMapData.currentMissionID = sz_currentMissionID;
        _galaxyMapData.position.Set(sz_positionX, sz_positionY, sz_positionZ);
    }
}

