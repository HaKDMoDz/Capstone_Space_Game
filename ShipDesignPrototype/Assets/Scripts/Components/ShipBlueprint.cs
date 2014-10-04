using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShipBlueprint
{

    #region Fields
    Hull hull;
    public Hull Hull
    {
        get { return hull; }
    }
    
    //List<ComponentSlot> componentGrid;

    private Dictionary<ComponentSlot, ShipComponent> componentTable;
    public Dictionary<ComponentSlot, ShipComponent> ComponentTable
    {
        get { return componentTable; }
    }

    
    
    public ShipBlueprint(Hull _hull)
    {
        componentTable = new Dictionary<ComponentSlot, ShipComponent>();
        hull = _hull;
        componentTable = new Dictionary<ComponentSlot, ShipComponent>();
        
    }

    #endregion

    #region Methods

    public void AddComponent(ShipComponent component, ComponentSlot slot)
    {
        componentTable.Add(slot, component);
        slot.installedComponent = component;
    }

    public void AddComponent(int slotIndex, ShipComponent component )
    {
        componentTable.Add(hull.SlotTable[slotIndex], component);
    }
    public void RemoveComponent(ShipComponent component, ComponentSlot slot)
    {

    }




    #endregion

}

