using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBlueprint 
{
    public Hull hull;
    public Dictionary<ComponentSlot, ShipComponent> componentTable;

    public ShipBlueprint(Hull _hull)
    {
        componentTable = new Dictionary<ComponentSlot, ShipComponent>();
        hull = _hull;
    }
}
