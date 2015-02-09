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
    /// <summary>
    /// Temp method for AI building
    /// </summary>
    /// <param name="shipType"></param>
    /// <param name="blueprint"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public TurnBasedUnit BuildShip(ShipType shipType, ShipBlueprint blueprint, Vector3 position, Quaternion rotation)
    {
        blueprintBeingBuilt = blueprint;
        return InstantiateShip(shipType, position, rotation); 
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
        #if FULL_DEBUG
        if (!hullBeingBuilt)
        {
            Debug.LogError("ship null");
        }
        #endif
        
        hullBeingBuilt.Init();

        TurnBasedUnit setupUnit;

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

        SetupScriptsOnShip(shipType, out setupUnit);

        return setupUnit;
        //return GetFullySetupShip(shipType);
    }

    private void SetupScriptsOnShip(ShipType shipType, out TurnBasedUnit setupUnit)
    {
        setupUnit = null;
        switch (shipType)
        {
            case ShipType.PlayerShip:
                PlayerShip playerShip =  hullBeingBuilt.gameObject.AddComponent<PlayerShip>();
                ShipMove shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                PlayerAttack playerAttack = hullBeingBuilt.gameObject.AddComponent<PlayerAttack>();
                playerShip.Init(blueprintBeingBuilt, shipMove, playerAttack);
                playerShip.gameObject.name = "Player " + playerShip.gameObject.name;
                setupUnit = playerShip;
                
                break;
            case ShipType.AI_Ship:
                AI_Ship ai_ship = hullBeingBuilt.gameObject.AddComponent<AI_Ship>();
                ShipMove ai_shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                AI_Attack ai_attack = hullBeingBuilt.gameObject.AddComponent<AI_Attack>();
                ai_ship.Init(blueprintBeingBuilt, ai_shipMove, ai_attack);
                ai_ship.gameObject.layer = TagsAndLayers.AI_ShipLayer;
                ai_ship.gameObject.name = "AI " + ai_ship.gameObject.name;
                setupUnit = ai_ship;
                break;
            case ShipType.NPC_Ship:
                break;
            default:
                break;
        }
    }

    #region UnityCallbacks
    

    #endregion UnityCallbacks

    #endregion Private

    #endregion Methods

}
