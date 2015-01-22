using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ShipType {PlayerShip, AI_Ship, NPC_Ship}

public class ShipBuilder
{
    #region Fields
    
    #region Internal

    //references
    private ShipBlueprintSaveSystem saveSystem;

    private ShipBlueprint blueprintBeingBuilt;
    private Hull hullBeingBuilt;
    #endregion Internal

    #endregion Fields

    #region Methods

    #region Public

    public ShipBuilder()
    {
        saveSystem = new ShipBlueprintSaveSystem();
    }

    public TurnBasedUnit BuildShip(ShipType shipType, string blueprintName, Vector3 position, Quaternion rotation)
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

        return InstantiateShip(shipType, position, rotation);
    }

    #endregion Public

    #region Private

    private TurnBasedUnit InstantiateShip(ShipType shipType, Vector3 position, Quaternion rotation)
    {
        hullBeingBuilt = GameObject.Instantiate(blueprintBeingBuilt.hull, position, rotation) as Hull;
        if (!hullBeingBuilt)
        {
            Debug.LogError("ship null");
        }

        hullBeingBuilt.Init();

        for (int i = 0; i < blueprintBeingBuilt.slot_component_table.Count; i++)
        {
            var slot_component = blueprintBeingBuilt.slot_component_table.ElementAt(i);
            int slotIndex = slot_component.Key.index;

#if !NO_DEBUG
            if (hullBeingBuilt.index_slot_table.ContainsKey(slotIndex))
            {
                Transform slotTrans = hullBeingBuilt.index_slot_table[slotIndex].transform;
                ShipComponent builtComponent = GameObject.Instantiate(slot_component.Value, slotTrans.position, slotTrans.rotation) as ShipComponent;
                blueprintBeingBuilt.slot_component_table[slot_component.Key] = builtComponent;
                builtComponent.transform.SetParent(slotTrans, true);
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

        return GetFullySetupShip(shipType);
    }

    private TurnBasedUnit GetFullySetupShip(ShipType shipType)
    {
        TurnBasedUnit setupUnit = null;
        switch (shipType)
        {
            case ShipType.PlayerShip:
                PlayerShip playerShip =  hullBeingBuilt.gameObject.AddComponent<PlayerShip>();
                ShipMove shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                ShipControlInterface shipControlInterface = hullBeingBuilt.gameObject.AddComponent<ShipControlInterface>();
                playerShip.Init(blueprintBeingBuilt, shipMove, shipControlInterface);
                setupUnit = playerShip;
                break;
            case ShipType.AI_Ship:

                ShipMove ai_shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                break;
            case ShipType.NPC_Ship:
                break;
            default:
                break;
        }
        return setupUnit;
    }

    #region UnityCallbacks
    

    #endregion UnityCallbacks

    #endregion Private

    #endregion Methods

}
