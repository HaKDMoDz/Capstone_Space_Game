/*
  ShipBuilder.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 8/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
#if FULL_DEBUG
        //if (shipType == ShipType.AI_Ship)
        //if(false)
        //{
        //    Debug.LogError("BEFORE ... blueprint " + blueprintBeingBuilt);
        //    Debug.LogError("hull.emptyCompGrid: ");
        //    foreach (ComponentSlot slot in blueprintBeingBuilt.hull.EmptyComponentGrid)
        //    {
        //        Debug.LogError("ComponentSlot: " + slot.index + ": " + slot.InstalledComponent);

        //    }
        //    Debug.LogError("hull.index_slot_table: ");
        //    foreach (var item in blueprintBeingBuilt.slot_component_table)
        //    {
        //        Debug.LogError("slot: " + item.Key + " comp: " + item.Value);
        //    }
        //}
        
#endif


        return InstantiateShip(false, shipType, position, rotation); 
    }

    public TurnBasedUnit BuildShip(ShipType shipType, BlueprintTemplate bpTemplate, Vector3 position, Quaternion rotation)
    {
        hullBeingBuilt = GameObject.Instantiate(bpTemplate.Hull, position, rotation) as Hull;
        #if FULL_DEBUG ||LOW_DEBUG
        if (!hullBeingBuilt)
        {
            Debug.LogError("ship null");
        }
        #endif
        hullBeingBuilt.Init();

        bpTemplate.GetBlueprint(out blueprintBeingBuilt, hullBeingBuilt);
        
        return InstantiateShip(true, shipType, position, rotation);
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

        return InstantiateShip(false, shipType, position, rotation);
    }

    #endregion Public

    #region Private

    private TurnBasedUnit InstantiateShip(bool hullSpawnedAlready, ShipType shipType, Vector3 position, Quaternion rotation)
    {
        if (!hullSpawnedAlready)
        {
            hullBeingBuilt = GameObject.Instantiate(blueprintBeingBuilt.Hull, position, rotation) as Hull;
            #if FULL_DEBUG ||LOW_DEBUG
            if (!hullBeingBuilt)
            {
                Debug.LogError("ship null");
            }
            #endif
            hullBeingBuilt.Init();
        }
       
        for (int i = 0; i < blueprintBeingBuilt.Slot_component_table.Count; i++)
        {
            var slot_component = blueprintBeingBuilt.Slot_component_table.ElementAt(i);
            int slotIndex = slot_component.Key.index;

#if !NO_DEBUG
            if (hullBeingBuilt.index_slot_table.ContainsKey(slotIndex))
            {
                Transform slotTrans = hullBeingBuilt.index_slot_table[slotIndex].transform;
                ShipComponent builtComponent = GameObject.Instantiate(slot_component.Value, slotTrans.position, slotTrans.rotation) as ShipComponent;
                blueprintBeingBuilt.Slot_component_table[slot_component.Key] = builtComponent;
                builtComponent.transform.SetParent(slotTrans, true);
                builtComponent.Placement = slot_component.Key.Placement;
                slot_component.Key.InstalledComponent = slot_component.Value;
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

        return SetupScriptsOnShip(shipType);
    }

    private TurnBasedUnit SetupScriptsOnShip(ShipType shipType)
    {
        switch (shipType)
        {
            case ShipType.PlayerShip:
                //PlayerShip_Old playerShip = hullBeingBuilt.gameObject.AddComponent<PlayerShip_Old>();
                //ShipMove shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                //PlayerAttack playerAttack = hullBeingBuilt.gameObject.AddComponent<PlayerAttack>();
                //playerShip.Init(blueprintBeingBuilt, shipMove, playerAttack);

                PlayerShip playerShip = hullBeingBuilt.gameObject.AddComponent<PlayerShip>();
                ShipMove shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                playerShip.Init(blueprintBeingBuilt, shipMove);

                playerShip.gameObject.name = "Player " + blueprintBeingBuilt.MetaData.BlueprintName;
                
                return playerShip;
                
            case ShipType.AI_Ship:
                AI_Ship ai_ship = hullBeingBuilt.gameObject.AddComponent<AI_Ship>();
                ShipMove ai_shipMove = hullBeingBuilt.gameObject.AddComponent<ShipMove>();
                AI_Attack ai_attack = hullBeingBuilt.gameObject.AddComponent<AI_Attack>();
                ai_ship.Init(blueprintBeingBuilt, ai_shipMove, ai_attack);
                ai_ship.gameObject.layer = TagsAndLayers.AI_ShipLayer;
                ai_ship.gameObject.name = blueprintBeingBuilt.MetaData.BlueprintName;
                
                return ai_ship;

            case ShipType.NPC_Ship:
                break;
            default:
                break;
        }
        return null;
    }

    #region UnityCallbacks
    

    #endregion UnityCallbacks

    #endregion Private

    #endregion Methods

}
