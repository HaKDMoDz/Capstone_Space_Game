using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AI_Data
{
    SerializedAI_Data Serialized()
    {
        return new SerializedAI_Data();
    }
}

[Serializable]
public class SerializedAI_Data
{
    AI_Data DeSerialized()
    {
        return new AI_Data();
    }
}
