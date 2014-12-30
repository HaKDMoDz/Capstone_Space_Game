using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBlueprint
{
    #region Fields
    public Hull hull { get; set; }
    public Dictionary<ComponentSlot, ShipComponent> slot_component_table { get; private set; }
    #endregion Fields

    #region Methods

    public ShipBlueprint() 
    {
        slot_component_table = new Dictionary<ComponentSlot, ShipComponent>();
    }
    public ShipBlueprint(Hull hull)
    {
        slot_component_table = new Dictionary<ComponentSlot, ShipComponent>();
        this.hull = hull;
    }

    public void AddComponent(ComponentSlot slot, ShipComponent component)
    {
#if !NO_DEBUG
        if(slot_component_table.ContainsKey(slot))
        {
            #if FULL_DEBUG
            Debug.LogError("Slot " + slot.index + " already has component " + slot_component_table[slot].componentName + " installed...replacing");
            #endif
            slot_component_table[slot] = component;
        }
        else
        {
            slot_component_table.Add(slot, component);
        }
        slot.InstalledComponent = component;
#else
        slot_component_table.Add(slot, component);
        slot.InstalledComponent = component;
#endif
    }//AddComponent

    public void RemoveComponent(ComponentSlot slot)
    {
#if !NO_DEBUG
        if(slot_component_table.ContainsKey(slot))
        {
            slot_component_table.Remove(slot);
            slot.InstalledComponent = null;
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("slot " + slot.index + " is not populated in the blueprint");
            #endif
        }

#else
        slot_component_table.Remove(slot);
        //slot.InstalledComponent = null;
#endif
    }//RemoveComponent

    public void Clear()
    {
        hull = null;
        slot_component_table.Clear();
    }

    #endregion Methods

}
