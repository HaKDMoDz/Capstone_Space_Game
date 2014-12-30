using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

#region AdditionalStructs
[Serializable]
public struct ShipBPSaveFields
{
    public string fileExtension_ShipBP;
    public string saveDirectory_ShipBP;
    public string fileName_SaveList;
}
#endregion AdditionalStructs

public class ShipDesignSystem : Singleton<ShipDesignSystem>
{
    #region Fields
    #region Private
    #region EditorExposed

    [SerializeField]
    private HullTable hullTableScriptableObject;
    [SerializeField]
    private ComponentTable compTableScriptableObject;
    [SerializeField]
    private ShipBPSaveFields saveFields;
    #endregion EditorExposed

    #region Internal
    //References
    ShipBlueprintSaveSystem saveSystem;

    //database references
    //public Dictionary<int, Hull> id_hull_table { get; private set; }
    //public Dictionary<Hull, int> hull_id_table { get; private set; }
    //public Dictionary<int, ShipComponent> id_comp_table { get; private set; }
    //public Dictionary<ShipComponent, int> comp_id_table { get; private set; }

    private bool buildingShip;
    private ShipBlueprint blueprintBeingBuilt;
    private Hull hullBeingBuilt;
    private List<ShipComponent> componentsBeingBuilt;
    //Dictionary<ComponentSlot, ShipComponent> slot_componentsBeingBuilt_table;
    #endregion Internal
    #endregion Private
    #endregion Fields

    #region Methods
    #region Public
    public void BuildHull(int hull_ID)
    {
        if (!buildingShip)
        {
            blueprintBeingBuilt.Clear();
            blueprintBeingBuilt.hull = HullTable.GetHull(hull_ID);
            //blueprintBeingBuilt.hull = id_hull_table[hull_ID];
            #if FULL_DEBUG
            Debug.Log("Building hull: " + blueprintBeingBuilt.hull.hullName);
            #endif
            AddHullToScene(blueprintBeingBuilt.hull);
            buildingShip = true;
        }
        else
        {
            #if FULL_DEBUG
            Debug.Log("Already building a ship");
            #endif
        }
    }//BuildHull

    public void BuildComponent(ComponentSlot slot, ShipComponent component)
    {
        #if FULL_DEBUG
        Debug.Log("Building component " + component.componentName + " on slot " + slot.index);
        #endif
#if !NO_DEBUG
        if(buildingShip)
        {
            AddComponentToScene(slot, component);
            blueprintBeingBuilt.AddComponent(slot, component);
        }
        else
        {
            #if FULL_DEBUG || LOW_DEBUG
            Debug.LogError("No ship being built");
            #endif
        }
#else
        AddComponentToScene(slot, component);
        blueprintBeingBuilt.AddComponent(slot, component);
#endif
    }//BuildComponent

    public void ClearScreen()
    {
        #if FULL_DEBUG
        Debug.Log("Clear Screen");
        #endif

        buildingShip = false;
        blueprintBeingBuilt.Clear();
        if (hullBeingBuilt)
        {
            Destroy(hullBeingBuilt.gameObject);
        }
        hullBeingBuilt = null;
        if (componentsBeingBuilt.Count > 0)
        {
            foreach (ShipComponent comp in componentsBeingBuilt)
            {
                Destroy(comp.gameObject);
            }
        }
        componentsBeingBuilt.Clear();

    }//ClearScreen

    #region SaveSystemInterface
    public void SaveBlueprint(string fileName)
    {
        saveSystem.SaveBlueprint(blueprintBeingBuilt, fileName);
    }
    public void LoadBlueprint(string fileName)
    {
        ClearScreen();
#if !NO_DEBUG
        if(saveSystem.LoadBlueprint(out blueprintBeingBuilt, fileName))
        {
            AddHullToScene(blueprintBeingBuilt.hull);
            foreach (var slot_component in blueprintBeingBuilt.slot_component_table)
            {
                int slotIndex = slot_component.Key.index;
                ComponentSlot slotToBuildOn = hullBeingBuilt.index_slot_table[slotIndex];
                AddComponentToScene(slotToBuildOn, slot_component.Value);
            }
            buildingShip = true;
        }
        else
        {
            Debug.LogError("ShipBlueprint named " + fileName + " could not be found");
        }
#else
        saveSystem.LoadBlueprint(out blueprintBeingBuilt, fileName);
        AddHullToScene(blueprintBeingBuilt.hull);
        foreach (var slot_component in blueprintBeingBuilt.slot_component_table)
        {
            AddComponentToScene(slot_component.Key, slot_component.Value);
        }
        buildingShip = true;
#endif
    }//LoadBlueprint

    public void DeleteBlueprint(string fileName)
    {
        #if FULL_DEBUG || LOW_DEBUG
        Debug.LogWarning("Deleting Blueprint " + fileName);
        #endif
        saveSystem.DeleteBlueprint(fileName);
    }
    public void DeleteAllBlueprints()
    {
        #if !NO_DEBUG
        Debug.LogWarning("Deleting All Blueprints ");
        #endif
        saveSystem.DeleteAllBlueprints();
    }
    #endregion SaveSystemInterface
    #endregion Public
    #region Private

    private void AddHullToScene(Hull hull)
    {
        hullBeingBuilt = Instantiate(hull, Vector3.zero, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as Hull;
        hullBeingBuilt.Init();
        //camera
    }

    private void AddComponentToScene(ComponentSlot slot, ShipComponent component)
    {
        Transform slotTrans = slot.transform;
        ShipComponent builtComp = Instantiate(component, slotTrans.position, slotTrans.rotation) as ShipComponent;
        componentsBeingBuilt.Add(builtComp);
    }

    #region UnityCallBacks
    private void Awake()
    {
        //generating tables on awake - is accessed from other scripts
        //id_hull_table = hullTableScriptableObject.Hull_id_List
        //    .ToDictionary(h => h.ID, h => h.hull);
        //hull_id_table = hullTableScriptableObject.Hull_id_List
        //    .ToDictionary(h => h.hull, h => h.ID);
        //id_comp_table = compTableScriptableObject.Comp_id_List
        //    .ToDictionary(c => c.ID, c => c.component);
        //comp_id_table = compTableScriptableObject.Comp_id_List
        //    .ToDictionary(c => c.component, c => c.ID);
        hullTableScriptableObject.Init();
        compTableScriptableObject.Init();

        componentsBeingBuilt = new List<ShipComponent>();
        blueprintBeingBuilt = new ShipBlueprint();
        saveSystem = new ShipBlueprintSaveSystem(saveFields.fileExtension_ShipBP, saveFields.saveDirectory_ShipBP, saveFields.fileName_SaveList);
    }

    #endregion UnityCallBacks


    #endregion Private
    #endregion Methods

}
