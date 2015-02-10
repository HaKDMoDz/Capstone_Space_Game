using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour 
{
    private AI_Fleet.Population pop1;

    public Comp_Wpn_Laser laser;
    public Comp_Pwr_PowerPlant powerPlant;
    public Comp_Def_Shield shield;
    public Comp_Def_Armour armour;
    public Comp_Eng_Thruster thruster;
    public Comp_Sup_Scanner scanner;
    public Comp_Wpn_Fighter_Bay fighterBay;
    public Comp_Wpn_Flak_Cannon flakCannon;
    public Comp_Wpn_Missile missile;
    public Comp_Wpn_Railgun railgun;
    public Comp_Wpn_Repair_Beam repairBeam;

    public static List<ShipBlueprint> blueprints;

    private ShipBuilder shipBuilder;

	void Awake () 
    {
        AI_Fleet.RandomManager.InitializeManager();
        pop1 = new AI_Fleet.Population(10, this);
        blueprints = pop1.GenerateBluePrints();

        Vector3 aiSpawnPos = new Vector3(0, 0, 100);
        int spawnSpacing = 50;

        shipBuilder = new ShipBuilder();
        //build random AI fleet
        foreach (ShipBlueprint sbp in AIManager.blueprints)
        {
            TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.AI_Ship, sbp, aiSpawnPos, Quaternion.identity);
            TurnBasedCombatSystem.Instance.AddShip(unit);
            aiSpawnPos.x -= spawnSpacing;
            unit.transform.RotateAroundYAxis(180.0f);
        }

	}
}
