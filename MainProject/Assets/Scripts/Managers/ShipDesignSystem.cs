using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipDesignSystem : Singleton<ShipDesignSystem>
{
    #region Fields
    #region Private
    #region EditorExposed

    [SerializeField]
    private HullTable hullTableScriptableObject;
    [SerializeField]
    private ComponentTable compTableScriptableObject;

    #endregion EditorExposed

    #region Internal

    public Dictionary<int, Hull> id_hull_table { get; private set; }
    public Dictionary<int, ShipComponent> id_comp_table { get; private set; }

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
        #if FULL_DEBUG
        Debug.Log("Building hull: " + id_hull_table[hull_ID].hullName);
        #endif

        if (!buildingShip)
        {
            AddHullToScene(id_hull_table[hull_ID]);
            hullBeingBuilt.Init();
            blueprintBeingBuilt.Clear();
            blueprintBeingBuilt.hull = id_hull_table[hull_ID];
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
    #endregion Public
    #region Private

    private void AddHullToScene(Hull hull)
    {
        hullBeingBuilt = Instantiate(hull, Vector3.zero, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as Hull;
        //camera
    }

    private void AddComponentToScene(ComponentSlot slot, ShipComponent component)
    {
        ShipComponent builtComp = Instantiate(component, slot.transform.position, slot.transform.rotation) as ShipComponent;
        componentsBeingBuilt.Add(builtComp);
    }

    #region UnityCallBacks
    private void Awake()
    {
        //generating tables on awake - is accessed from other scripts
        id_hull_table = hullTableScriptableObject.Hull_id_List
            .ToDictionary(h => h.ID, h => h.hull);
        id_comp_table = compTableScriptableObject.Comp_id_List
            .ToDictionary(c => c.ID, c => c.component);

        componentsBeingBuilt = new List<ShipComponent>();
        blueprintBeingBuilt = new ShipBlueprint();
    }

    #endregion UnityCallBacks


    #endregion Private
    #endregion Methods

}
