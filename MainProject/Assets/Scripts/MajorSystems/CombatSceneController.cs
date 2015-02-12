using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneController : Singleton<CombatSceneController>
{
    #region Fields


    #region Internal

    //references
    private PlayerFleetData playerFleetData;
    private AI_Data pirateFleetData;
    private ShipBuilder shipBuilder;
    #endregion Internal

    #endregion Fields

    #region Methods

    #region PrivateMethods

    /// <summary>
    /// Setups up the combat scene: adds ships, background objects, etc., and then tells the turn based system to start combat
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupScene()
    {
        #if FULL_DEBUG
        Debug.Log("Setup Combat Scene");
        #endif

        //setup background objects, etc.

        //get the saved player fleet
        playerFleetData = GameController.Instance.GameData.playerFleetData;
        pirateFleetData = GameController.Instance.GameData.pirates_AI_Data;

        

        #if FULL_DEBUG
        if(playerFleetData.currentFleet_BlueprintNames.Count==0)
        {
            Debug.LogError("Empty player fleet");
        }
	    #endif

        #if FULL_DEBUG
        //if (pirateFleetData.currentFleet_BlueprintNames.Count == 0)
        //{
        //    Debug.LogError("Empty enemy fleet");
        //}
        #endif

        /////positioning ships automatically for now
        Vector3 spawnPos = new Vector3(0, 0, -100);
        Vector3 aiSpawnPos = new Vector3(0,0,100);
        int numShips = playerFleetData.currentFleet_BlueprintNames.Count;
        int spawnSpacing = 50;
        spawnPos.x -= spawnSpacing * numShips / 2;
        /////////////////////////////////////////////////

        TurnBasedCombatSystem.Instance.Init();

        //tells the shipbuilder to build each ship in the fleet data
        foreach (string blueprintName in playerFleetData.currentFleet_BlueprintNames)
        {
            TurnBasedCombatSystem.Instance.AddShip(shipBuilder.BuildShip(ShipType.PlayerShip, blueprintName, spawnPos, Quaternion.identity));
            spawnPos.x += spawnSpacing;
        }

        //AIManager.Instance.Init(shipBuilder);
        //build AI fleet
        //pirate fleet init code

        //List<string> ai_ships = new List<string> { "AI_Corvette", "AI_Frigate"};
        
        //pirateFleetData.currentFleet_BlueprintNames = ai_ships;

       // Debug.LogError(pirateFleetData.currentFleet_BlueprintNames.Count);

        foreach (string bpTemplateName in pirateFleetData.currentFleet_BlueprintNames)
        //foreach (string bpTemplateName in ai_ships)
        {
            TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.AI_Ship, BlueprintTemplates.GetBPTemplate(bpTemplateName), aiSpawnPos, Quaternion.identity);
            #if FULL_DEBUG
            if (unit == null)
            {
                Debug.Log("shipbuilder returned null");
            }
            #endif

            TurnBasedCombatSystem.Instance.AddShip(unit);
            aiSpawnPos.x -= spawnSpacing;
            unit.transform.RotateAroundYAxis(180.0f);
        }



        //combat start 
        //comment out this line for the combat system tester to work
        yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
        yield return null;
    }//SetupScene

    private void Init()
    {
        shipBuilder = new ShipBuilder();
    }

    #region UnityCallbacks
    
    private IEnumerator Start()
    {
        Init();

        yield return StartCoroutine(SetupScene());
    }
    
    #endregion UnityCallbacks


    #endregion PrivateMethods

    #endregion Methods
}
