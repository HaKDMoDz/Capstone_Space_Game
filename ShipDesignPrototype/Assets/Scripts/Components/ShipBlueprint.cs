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
        //if(componentTable == null)
        //{
        //    Debug.Log("null table");
        //}
        //if(component == null)
        //{
        //    Debug.Log("null component");
        //}
        //if(hull.SlotTable == null)
        //{
        //    Debug.Log("slot table null");
        //}
        //Debug.Log("Adding comp: index: " + slotIndex + "slot: " + hull.SlotTable[slotIndex].index);
        componentTable.Add(hull.SlotTable[slotIndex], component);
    }
    public void RemoveComponent(ComponentSlot slot)
    {
        componentTable.Remove(slot);
        slot.installedComponent = null;
    }
    public void OutputContents()
    {
        if (componentTable != null)
        {
            Debug.Log("Hull: " + Hull.name  + "ID: " + Hull.ID);
            foreach (var item in componentTable)
            {
                Debug.Log(item.Key.index + ": " + item.Value.componentName);
            }
        }
        else
        {
            Debug.Log("Shipblueprint is null");
        }
    }


    #endregion

}

