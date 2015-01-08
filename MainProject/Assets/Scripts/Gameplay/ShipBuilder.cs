using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBuilder// : Singleton<ShipBuilder> 
{
    #region Fields
    
    #region Internal

    //references
    private ShipBlueprintSaveSystem saveSystem;

    private ShipBlueprint blueprintBeingBuilt;
    //private GameObject shipBeingBuilt;
    private Hull hullBeingBuilt;
    #endregion Internal

    #endregion Fields

    #region Methods

    #region Public

    public ShipBuilder()
    {
        saveSystem = new ShipBlueprintSaveSystem();
    }

    public GameObject BuildShip(string blueprintName, Vector3 position, Quaternion rotation)
    {
        #if !NO_DEBUG
        if (saveSystem.LoadBlueprint(out blueprintBeingBuilt, blueprintName))
        {
            Debug.Log("building " + blueprintName);
        }
        else
        {
            Debug.LogError("Blueprint " + blueprintName + " not found");
            return null;
        }
        #else
        saveSystem.LoadBlueprint(out blueprintBeingBuilt, blueprintName)
        #endif

        return InstantiateShip(position, rotation);
    }

    #endregion Public

    #region Private

    private GameObject InstantiateShip(Vector3 position, Quaternion rotation)
    {
        //shipBeingBuilt = Instantiate(blueprintBeingBuilt.hull, position, rotation) as GameObject;
        hullBeingBuilt = GameObject.Instantiate(blueprintBeingBuilt.hull, position, rotation) as Hull;
        if (!hullBeingBuilt)
        {
            Debug.LogError("ship null");
        }
        //Hull hullBeingBuilt = shipBeingBuilt.GetComponent<Hull>();

        hullBeingBuilt.Init();

        foreach (var slot_component in blueprintBeingBuilt.slot_component_table)
        {
            int slotIndex = slot_component.Key.index;

#if !NO_DEBUG
            if (hullBeingBuilt.index_slot_table.ContainsKey(slotIndex))
            {
                Transform slotTrans = hullBeingBuilt.index_slot_table[slotIndex].transform;
                ShipComponent component = GameObject.Instantiate(slot_component.Value, slotTrans.position, slotTrans.rotation) as ShipComponent;
                component.transform.SetParent(slotTrans, true);
            }
            else
            {
                Debug.LogError("Slot " + slotIndex + " not found in Hull " + hullBeingBuilt.hullName);
            }
#else   
            Transform slotTrans = hullBeingBuilt.index_slot_table[slotIndex].transform;
            GameObject component = GameObject.Instantiate(slot_component.Value, slotTrans.position, slotTrans.rotation) as GameObject;
            component.transform.SetParent(slotTrans, true);
#endif
        }

        return hullBeingBuilt.gameObject;
    }

    #region UnityCallbacks
    //private void Awake()
    //{
    //}
    #endregion UnityCallbacks

    #endregion Private

    #endregion Methods

}
