using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShipBlueprintSaveSystem : MonoBehaviour 
{

    public void Save(ShipBlueprint shipBP)
    {
        
    }
    public bool Load(out ShipBlueprint shipBP)
    {
        shipBP = null;
        return false;
    }

    SerializedShipBlueprint SerializeShipBP(ShipBlueprint ship)
    {
        return null;
    }

    ShipBlueprint DeserializeShipBP(SerializedShipBlueprint shipBP)
    {
        return null;
    }

}
[Serializable]
public class SerializedShipBlueprint
{
    int hull_ID;
    Dictionary<int, SerializedComponent> componentTable;

    public SerializedShipBlueprint(int _hullID, Dictionary<int, SerializedComponent> _componentTable)
    {
        hull_ID = _hullID;
        componentTable = _componentTable;
    }

}
[Serializable]
public class SerializedComponent
{
    int ID;
    ShipComponent.ComponentType type;

    public SerializedComponent(int _ID, ShipComponent.ComponentType _type)
    {
        ID = _ID;
        type = _type;
    }
}