/*
  ShipBlueprint.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 22/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
#endregion Usings

[Serializable]
public class ShipBlueprint
{
    #region Fields
    [SerializeField]
    private Hull hull;

    public Hull Hull
    {
        get { return hull; }
        set { hull = value; }
    }
    private Dictionary<ComponentSlot, ShipComponent> slot_component_table = new Dictionary<ComponentSlot, ShipComponent>();
    public Dictionary<ComponentSlot, ShipComponent> Slot_component_table
    {
        get { return slot_component_table; }
    }
    [SerializeField]
    private ShipBlueprintMetaData metaData;

    public ShipBlueprintMetaData MetaData
    {
        get { return metaData; }
        set { metaData = value; }
    }
    
    #endregion Fields

    #region Methods

    /// <summary>
    /// This constuctor should only be used for temporary initialization of a ShipBlueprint that is going to have a full blueprint assigned into it. 
    /// The ShipBlueprint(Hull) constructor should be used to instantiate a ShipBlueprint ready for use by passing in the Hull to build the blueprint with.
    /// </summary>
    public ShipBlueprint() 
    {
        Init();
    }
    
    public ShipBlueprint(Hull hull)
    {
        Init();
        this.hull = hull;
    }

    /// <summary>
    /// Adds the specified component to the specified slot on the shipblueprint. Error checking should be done in advance to make sure the current slot is empty so a new component can be installed on it.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="component"></param>
    public void AddComponent(ComponentSlot slot, ShipComponent component)
    {
#if FULL_DEBUG || LOW_DEBUG
        if(slot_component_table.ContainsKey(slot))
        {
            #if FULL_DEBUG
            Debug.LogError("Slot " + slot.index + " already has component " + slot_component_table[slot].componentName 
                + " installed. We should explicitly clear the slot before installing a different component");
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
    public void RemoveComponent(int slotIndex)
    {
        ComponentSlot slot = slot_component_table.Keys.FirstOrDefault(s => s.index == slotIndex);
        #if FULL_DEBUG
        if (!slot)
        {
            Debug.LogError("No slot found with index " + slotIndex);
        }
        else
        #endif
        {
            slot_component_table.Remove(slot);
            slot.InstalledComponent = null;
        }
    }
    public void RemoveComponent(ComponentSlot slot)
    {
#if !NO_DEBUG
        if(slot_component_table.ContainsKey(slot))
        {
            slot_component_table.Remove(slot);
            slot.InstalledComponent = null;
        }
        #if FULL_DEBUG
        else
        {
            Debug.LogError("slot " + slot.index + " is not populated in the blueprint");
        }
        #endif

#else
        slot_component_table.Remove(slot);
        //slot.InstalledComponent = null;
#endif
    }//RemoveComponent
    public bool IsValid()
    {
        GenerateMetaData();
        return (metaData.ExcessPower > 0.0f && metaData.MoveCost > 0.0f);
    }
    public void GenerateMetaData()
    {
        metaData.ExcessPower = CalculateExcessPower();
        metaData.MoveCost = CalculateMoveCost();
        metaData.FleetCost = CalculateFleetCost();
    }

    public void GenerateMetaData(string blueprintName)
    {
        metaData.BlueprintName = blueprintName;
        GenerateMetaData();
    }
    public int CalculateFleetCost()
    {
        return hull.EmptyComponentGrid.Count;
    }

    public float CalculateExcessPower()
    {
        float excessPower = 0.0f;
        foreach (ShipComponent component in slot_component_table.Values)
        {
            excessPower -= component.PowerDrain;
        }
        return excessPower;
    }
    public float CalculateMoveCost()
    {
        int numThrusters = slot_component_table.Values.Count(c => c is Comp_Eng_Thruster);
        #if FULL_DEBUG || LOW_DEBUG
        if (numThrusters <= 0)
        {
            return 0.0f;
        }
        #endif
        float thrust = ((Comp_Eng_Thruster)(slot_component_table.Values).FirstOrDefault(c => c is Comp_Eng_Thruster)).Thrust;
        float totalThrust = thrust * numThrusters;
        int mass = Hull.EmptyComponentGrid.Count;
        return (mass / totalThrust * 1.5f);
    }
    public void Clear()
    {
        hull = null;
        slot_component_table.Clear();
        metaData.Reset();   
    }

    private void Init()
    {
        metaData = new ShipBlueprintMetaData();
    }

    #if FULL_DEBUG
    public void Display()
    {
        Debug.Log("Contents of blueprint");
        Debug.Log("Hull : " + hull.hullName);
        foreach (var slot_comp in slot_component_table)
        {
            Debug.Log("Slot " + slot_comp.Key + " Comp " + slot_comp.Value);
        }
    }
    #endif
    #endregion Methods
}

#region AdditionalStructs
[Serializable]
public class ShipBlueprintMetaData
{
    [SerializeField]
    private string blueprintName;
    public string BlueprintName
    {
        get { return blueprintName; }
        set {blueprintName = value; }
    }
    //public string BlueprintName;
    public float ExcessPower;
    public float MoveCost;
    public int FleetCost;

    public ShipBlueprintMetaData()
    {
        Reset();
    }
    public ShipBlueprintMetaData(string blueprintName, float excessPower, float moveCost, int fleetCost)
    {
        this.BlueprintName = blueprintName;
        this.ExcessPower = excessPower;
        this.MoveCost = moveCost;
        this.FleetCost = fleetCost;
    }
    public ShipBlueprintMetaData(ShipBlueprintMetaData metaData)
    {
        this.BlueprintName = metaData.BlueprintName;
        this.ExcessPower = metaData.ExcessPower;
        this.MoveCost = metaData.MoveCost;
        this.FleetCost = metaData.FleetCost;
    }
    public void Reset()
    {
        //Debug.Log("Rest meta data");
        BlueprintName = "";
        ExcessPower = 0.0f;
        MoveCost = 0.0f;
        FleetCost = 0;
    }
}
#if FULL_DEBUG || LOW_DEBUG
[Serializable]
public class SlotIndex_CompID
{
    public int slotIndex;
    public int compID;

    public SlotIndex_CompID()
    {

    }
    public SlotIndex_CompID(int slotIndex, int compID)
    {
        this.slotIndex = slotIndex;
        this.compID = compID;
    }
}
#endif
[Serializable]
public class SerializedShipBlueprint //serializable version of the ShipBlueprint
{
    public int hull_ID;
    #if FULL_DEBUG || LOW_DEBUG
    public List<SlotIndex_CompID> slotIndex_CompID_Table;
    #else
    public Dictionary<int, int> slotIndex_CompID_Table;
    #endif

    public ShipBlueprintMetaData metaData ;

    public SerializedShipBlueprint()
    {
        hull_ID = -1;
        Init();
    }
    public SerializedShipBlueprint(int hull_ID)
    {
        this.hull_ID = hull_ID;
        Init();
    }
    private void Init()
    {
        #if FULL_DEBUG || LOW_DEBUG
        slotIndex_CompID_Table = new List<SlotIndex_CompID>();
        #else
        slotIndex_CompID_Table = new Dictionary<int, int>();
        #endif
    }
    public void AddComponent(int slotIndex, int compID)
    {
        #if FULL_DEBUG || LOW_DEBUG
        slotIndex_CompID_Table.Add(new SlotIndex_CompID(slotIndex, compID));
        #else
        slotIndex_CompID_Table.Add(slotIndex, compID);
        #endif
    }
    public void RemoveComponent(int slotIndex)
    {

    #if !NO_DEBUG
        #if FULL_DEBUG || LOW_DEBUG
        if(slotIndex_CompID_Table.Exists(s=>s.slotIndex == slotIndex))
        {
            int indexToRemove = slotIndex_CompID_Table.FindIndex(s=>s.slotIndex==slotIndex);
            slotIndex_CompID_Table.RemoveAt(indexToRemove);  
        }
        #else
        if (slotIndex_CompID_Table.ContainsKey(slotIndex))
        {
            slotIndex_CompID_Table.Remove(slotIndex);
        }
        #endif
        else
        {
            Debug.Log("slot " + slotIndex + " is not populated in the blueprint");
        }
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