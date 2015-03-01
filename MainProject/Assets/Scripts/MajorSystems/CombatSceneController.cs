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

        #if FULL_DEBUG
        if (pirateFleetData.currentFleet_BlueprintNames.Count == 0)
        {
            Debug.LogWarning("Empty enemy fleet - spawning default fleet");
            pirateFleetData.currentFleet_BlueprintNames = new List<string>() { "AI_Corvette", "AI_Frigate" };
        }
        #endif

        foreach (string bpTemplateName in pirateFleetData.currentFleet_BlueprintNames)
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
        #if UNITY_EDITOR
        if (!GameObject.Find("CombatSystemTester"))
        {
            yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
        }
        else
        {
            yield return null;
        }
        #else
        yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
        #endif
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
