/*
  CombatSceneController.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 8/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        /////positioning ships automatically for now
        Vector3 spawnPos = new Vector3(0, 0, -100);
        Vector3 aiSpawnPos = new Vector3(0,0,100);
        int numShips = playerFleetData.gridIndex_metaData_table.Count;
        int spawnSpacing = 50;
        spawnPos.x -= spawnSpacing * numShips / 2;
        /////////////////////////////////////////////////

        TurnBasedCombatSystem.Instance.Init();

        if (numShips == 0)
        {
            Debug.LogWarning("Empty player fleet - spawning default fleet");
            foreach (string bpTemplateName in new List<string>() { "DefaultCorvette", "DefaultCorvette" })
            {
                TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.PlayerShip, BlueprintTemplates.GetBPTemplate(bpTemplateName), spawnPos, Quaternion.identity);
                #if FULL_DEBUG
                if (unit == null)
                {
                    Debug.Log("shipbuilder returned null");
                }
                #endif
                TurnBasedCombatSystem.Instance.AddShip(unit);
                spawnPos.x += spawnSpacing;
            }
        }
        else
        {//tells the shipbuilder to build each ship in the fleet data
            foreach (string blueprintName in playerFleetData.gridIndex_metaData_table.Values.Select(meta=>meta.BlueprintName))
            {
                TurnBasedCombatSystem.Instance.AddShip(shipBuilder.BuildShip(ShipType.PlayerShip, blueprintName, spawnPos, Quaternion.identity));
                spawnPos.x += spawnSpacing;
            }
        }
        if (pirateFleetData.currentFleet_BlueprintNames.Count == 0)
        {
            Debug.LogWarning("Empty enemy fleet - spawning default fleet");
            pirateFleetData.currentFleet_BlueprintNames = new List<string>() { "AI_Corvette", "AI_Frigate" };
        }

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
