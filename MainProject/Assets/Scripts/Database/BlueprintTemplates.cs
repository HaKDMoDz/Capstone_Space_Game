/*
  BlueprintTemplates.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 11/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class BlueprintTemplates : ScriptableObject
{
    #region Fields
    [SerializeField]
    private List<BlueprintTemplate> bpTemplateList;
    public List<BlueprintTemplate> BpTemplateList
    {
        get { return bpTemplateList; }
    }

    //Database
    public static List<BlueprintTemplate> BlueprintTemplateList { get; private set; }
    public static Dictionary<string, BlueprintTemplate> Name_BpTemplate_Table {get; private set;}
    #endregion Fields

    #region Methods

    #region Public
    #region DatabaseAccess
    public static BlueprintTemplate GetBPTemplate(string name)
    {
#if FULL_DEBUG
        BlueprintTemplate bpTemplate=null;
        if(!Name_BpTemplate_Table.TryGetValue(name, out bpTemplate))
        {
            Debug.LogError("Blueprint Template named " + name + " not found");
        }
        return bpTemplate;
#else
        return Name_BpTemplate_Table[name];
#endif
    }

    #endregion DatabaseAccess

    #region GUIAccess
#if UNITY_EDITOR
    public bool BlueprintExists(string name)
    {
        return (BpTemplateList.Any(bp => bp.MetaData.BlueprintName == name));
    }

    public void AddBlueprint(string name)
    {
        Debug.Log("Adding blueprint " + name);
        ShipBlueprint blueprintBeingBuilt = typeof(ShipDesignSystem)
            .GetField("blueprintBeingBuilt", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(ShipDesignSystem.Instance) as ShipBlueprint;

        if (blueprintBeingBuilt == null)
        {
            Debug.LogError("Null blueprint");
        }
        if (blueprintBeingBuilt.Hull == null)
        {
            Debug.LogError("No Hull in blueprint");
        }
        if (blueprintBeingBuilt.Slot_component_table == null
            || blueprintBeingBuilt.Slot_component_table.Count == 0)
        {
            Debug.LogWarning("No components in blueprint");
        }
        blueprintBeingBuilt.GenerateMetaData(name);
        BpTemplateList.Add(new BlueprintTemplate(blueprintBeingBuilt));
    }
    public void RemoveBlueprint(string name)
    {
        Debug.Log("Remove blueprint " + name);
        if(BlueprintExists(name))
        {
            bpTemplateList.Remove(bpTemplateList.Find(b => b.MetaData.BlueprintName == name));
            BlueprintTemplateList.Remove(BlueprintTemplateList.Find(b => b.MetaData.BlueprintName == name));
        }
        else
        {
            Debug.LogError("Blueprint " + name + " not found");
        }
    }
    public void Wipe()
    {
        if (bpTemplateList != null)
        {
            bpTemplateList.Clear();
        }
    }
#endif

    #endregion GUIAccess
    #endregion Public
    #region UnityCallbacks
    private void OnEnable()
    {
        #if FULL_DEBUG || LOW_DEBUG
        if (bpTemplateList == null || bpTemplateList.Count == 0)
        {
            Debug.LogWarning("No Blueprint templates found");
        }
        #endif

        BlueprintTemplateList = bpTemplateList;
        Name_BpTemplate_Table = bpTemplateList.ToDictionary(bp=>bp.MetaData.BlueprintName, bp=>bp);

    }
    #endregion UnityCallbacks
    #endregion Methods

    

}
#region AdditionalStructs
[Serializable]
public class BlueprintTemplate
{
    [SerializeField]
    private Hull hull;
    public Hull Hull
    {
        get { return hull; }
    }

    [SerializeField]
    private ShipBlueprintMetaData metaData;
    public ShipBlueprintMetaData MetaData
    {
        get { return metaData; }
    }

    [SerializeField]
    private List<SlotIndexCompEntry> slotIndex_Comp_List = new List<SlotIndexCompEntry>();
    public List<SlotIndexCompEntry> SlotIndex_Comp_List
    {
        get { return slotIndex_Comp_List; }
    }

    /// <summary>
    /// Creates a BlueprintTemplate form a blueprint. Validation of blueprint should be done already.
    /// </summary>
    /// <param name="blueprint"></param>
    public BlueprintTemplate(ShipBlueprint blueprint)
    {
        this.hull = blueprint.Hull;
        foreach (var slot_comp in blueprint.Slot_component_table)
        {
            SlotIndex_Comp_List.Add(
                new SlotIndexCompEntry
                {
                    slotIndex = slot_comp.Key.index,
                    component = slot_comp.Value
                });
        }
        this.metaData = blueprint.MetaData;
    }

    public void GetBlueprint(out ShipBlueprint blueprint, Hull hullBeingBuilt)
    {
        blueprint = new ShipBlueprint(hull);
        foreach (var slotIndex_Comp in SlotIndex_Comp_List)
        {
            blueprint.AddComponent(hullBeingBuilt.index_slot_table[slotIndex_Comp.slotIndex], slotIndex_Comp.component);
        }
        blueprint.GenerateMetaData(MetaData.BlueprintName);
    }

    [Serializable]
    public struct SlotIndexCompEntry
    {
        public int slotIndex;
        public ShipComponent component;
    }
}



#endregion AdditionalStructs
