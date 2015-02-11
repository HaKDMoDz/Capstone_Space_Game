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

    #endregion Fields

    #region Methods

    #region Public
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
            Debug.LogError("No components in blueprint");
        }

        blueprintBeingBuilt.GenerateMetaData(name);
        BpTemplateList.Add(new BlueprintTemplate(blueprintBeingBuilt));
    }
    public void RemoveBlueprint(string name)
    {
        Debug.Log("Remove blueprint " + name);
    }
    public void Wipe()
    {
        if (bpTemplateList != null)
        {
            bpTemplateList.Clear();
        }
    }

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
        //set { hull = value; }
    }

    [SerializeField]
    private ShipBlueprintMetaData metaData;
    public ShipBlueprintMetaData MetaData
    {
        get { return metaData; }
        //set { metaData = value; }
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

    //public void GetBlueprint(ref ShipBlueprint blueprint)
    //{
    //    blueprint.Clear();
    //    blueprint.Hull = this.hull;
    //    blueprint.Slot_component_table = slotIndex_component_Table.ToDictionary(index_comp=>index_comp.)
    //}

    [Serializable]
    public struct SlotIndexCompEntry
    {
        public int slotIndex;
        public ShipComponent component;
    }
}



#endregion AdditionalStructs
