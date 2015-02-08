using UnityEngine;
using System.Collections;

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

    public Hull corvette, frigate, cruiser, battleship;

    public ComponentSlot componentSlot;

	// Use this for initialization
	void Awake () 
    {
        AI_Fleet.RandomManager.InitializeManager();
        pop1 = new AI_Fleet.Population(10, this);
	}

    public void OnMouseDown()
    {
        Debug.Log("AI_Debug");
        //pop1.DebugDisplay();

        pop1.GenerateBluePrints();
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
