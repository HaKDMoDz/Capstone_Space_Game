using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ShipBlueprint
{
    #region Fields
    //public string blueprintName;
    public Hull hull { get; set; }
    public Dictionary<ComponentSlot, ShipComponent> slot_component_table { get; private set; }
    public ShipBlueprintMetaData metaData { get; set; }
    #endregion Fields

    #region Methods

    public ShipBlueprint() 
    {
        Init();
    }
    public ShipBlueprint(Hull hull)
    {
        Init();
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
    public void GenerateMetaData()
    {
        metaData.excessPower = CalculateExcessPower();
    }
    public void GenerateMetaData(string blueprintName)
    {
        metaData.blueprintName = blueprintName;
        GenerateMetaData();
    }
    public float CalculateExcessPower()
    {
        float excessPower = 0.0f;
        foreach (ShipComponent component in slot_component_table.Values)
        {
            excessPower -= component.powerDrain;
        }
        return excessPower;
    }
    public void Clear()
    {
        hull = null;
        slot_component_table.Clear();
        metaData.Reset();   
    }

    private void Init()
    {
        slot_component_table = new Dictionary<ComponentSlot, ShipComponent>();
        metaData = new ShipBlueprintMetaData();
    }
    #endregion Methods

}

#region AdditionalStructs
[Serializable]
public class ShipBlueprintMetaData
{
    public string blueprintName { get; set; }
    public float excessPower { get; set; }

    public ShipBlueprintMetaData()
    {
        Reset();
    }
    public ShipBlueprintMetaData(string blueprintName, float excessPower)
    {
        this.blueprintName = blueprintName;
        this.excessPower = excessPower;
    }
    public void Reset()
    {
        blueprintName = "";
        excessPower = 0.0f;
    }
}
[Serializable]
public class SerializedShipBlueprint //serializable version of the ShipBlueprint
{
    public int hull_ID;
    public Dictionary<int, int> slotIndex_CompID_Table;
    public ShipBlueprintMetaData metaData ;

    public SerializedShipBlueprint(int hull_ID)
    {
        this.hull_ID = hull_ID;
        slotIndex_CompID_Table = new Dictionary<int, int>();
    }
    public void AddComponent(int slotIndex, int compID)
    {
        slotIndex_CompID_Table.Add(slotIndex, compID);
    }
    public void RemoveComponent(int slotIndex)
    {
#if !NO_DEBUG
        if (slotIndex_CompID_Table.ContainsKey(slotIndex))
        {
            slotIndex_CompID_Table.Remove(slotIndex);
        }
#if FULL_DEBUG
        else
        {
            Debug.Log("slot " + slotIndex + " is not populated in the blueprint");
        }
#endif
#else //NO_DEBUG
        slotIndex_CompID_Table.Remove(slotIndex);
#endif
    }//RemoveComp

    public void Clear()
    {
        hull_ID = -1;
        slotIndex_CompID_Table.Clear();
    }

}
#endregion AdditionalStructs