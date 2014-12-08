using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MissionData
{
    SerializedMissionData Serialized()
    {
        return new SerializedMissionData();
    }
}
[Serializable]
public class SerializedMissionData
{
    MissionData DeSerialized()
    {
        return new MissionData();
    }
}