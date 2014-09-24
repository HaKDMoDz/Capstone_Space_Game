using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBlueprint : MonoBehaviour 
{
    public Hull hull;
    public Dictionary<ComponentSlot, ShipComponent> componentTable;

    public void Init()
    {
        componentTable = new Dictionary<ComponentSlot, ShipComponent>();
    }

}
