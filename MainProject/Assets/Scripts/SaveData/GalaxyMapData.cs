using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GalaxyMapData 
{
    
    SerializedGalaxyMapData Serialized()
    {
        return new SerializedGalaxyMapData();
    }
}

[Serializable]
public class SerializedGalaxyMapData
{
    GalaxyMapData DeSerialized()
    {
        return new GalaxyMapData();
    }
}

