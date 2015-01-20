using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatSceneController : Singleton<CombatSceneController>
{
    #region Fields


    #region Internal

    //references
    private PlayerFleetData playerFleetData;
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
        
        #if FULL_DEBUG
        if(playerFleetData.currentFleet_BlueprintNames.Count==0)
        {
            Debug.LogError("Empty player fleet");
        }
	    #endif

        /////positioning ships automatically for now
        Vector3 spawnPos = Vector3.zero;
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

        //build AI fleet

        //combat start
        yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
        yield return null;
    }

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
