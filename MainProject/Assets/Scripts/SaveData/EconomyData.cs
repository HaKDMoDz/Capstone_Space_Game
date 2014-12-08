using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EconomyData 
{
    SerializedEconomyData Serialized()
    {
        return new SerializedEconomyData();
    }
}
[Serializable]
public class SerializedEconomyData
{
    EconomyData DeSerialized()
    {
        return new EconomyData();
    }
}
