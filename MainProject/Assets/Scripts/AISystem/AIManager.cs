﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : Singleton<AIManager>
{
    //Area for static vars used by AI systems
    public static float tgtClosest = 0.35f;
    public static float tgtFarthest = 0.5f;
    public static float tgtStrongest = 0.75f;
    public static float tgtWeakest = 0.85f;

    public static float cmpWeapon = 0.25f;
    public static float cmpDefence = 0.25f;
    public static float cmpEngineering = 0.25f;
    public static float cmpSupport = 0.25f;

    private AI_Fleet.Population pop1;

    public Comp_Wpn_Laser laser;
    public Comp_Eng_PowerPlant powerPlant;
    public Comp_Def_Shield shield;
    public Comp_Def_Armour armour;
    public Comp_Eng_Thruster thruster;
    public Comp_Sup_Scanner scanner;
    public Comp_Wpn_Fighter_Bay fighterBay;
    public Comp_Wpn_Flak_Cannon flakCannon;
    public Comp_Wpn_Missile missile;
    public Comp_Wpn_Railgun railgun;
    public Comp_Wpn_Repair_Beam repairBeam;

    public List<ShipBlueprint> blueprints;

    private ShipBuilder shipBuilder;

	public void Init (ShipBuilder _shipBuilder) 
    {
        blueprints = pop1.GenerateBluePrints();
        //pop1.DebugDisplay();

        Vector3 aiSpawnPos = new Vector3(0, 0, 100);
        int spawnSpacing = 50;

        shipBuilder = _shipBuilder;
        //build random AI fleet
        foreach (ShipBlueprint sbp in blueprints)
        {
            /*
            Debug.LogError("Before Build  ... blueprint " + sbp);
            Debug.LogError("hull.emptyCompGrid: ");
            Debug.LogError("componentCount: " + sbp.slot_component_table.Count);
            foreach (var slot in sbp.slot_component_table)
            {
                Debug.LogError("Slot: " + slot.Key + "Comp: " + slot.Value);

            }
            */
            TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.AI_Ship, sbp, aiSpawnPos, Quaternion.identity);
#if FULL_DEBUG
            /*
            Debug.LogError("AFTER ... blueprint " + sbp);
            Debug.LogError("hull.emptyCompGrid: ");
            Debug.LogError("componentCount: " + unit.Components.Count);
            foreach (ShipComponent slot in unit.Components)
            {
              //  Debug.LogError("ComponentSlot: " + slot);

            }*/
#endif
            TurnBasedCombatSystem.Instance.AddShip(unit);
            aiSpawnPos.x -= spawnSpacing;
            unit.transform.RotateAroundYAxis(180.0f);
        }

	}

}
