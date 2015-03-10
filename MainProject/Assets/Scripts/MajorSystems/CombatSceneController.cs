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
    //Editor Exposed
    [SerializeField]
    private Vector2 gridDimensions = new Vector2(50.0f, 62.5f);
    [SerializeField]
    private int gridWidth = 7;
    [SerializeField]
    private Vector3 playerStartSpawnPos = new Vector3(-150.0f, 0.0f,-250.0f);
    [SerializeField]
    private Vector3 aiStartSpawnPos = new Vector3(-150.0f,0.0f, 250.0f);
    //references
    private PlayerFleetData playerFleetData;
    private AI_Data pirateFleetData;
    private ShipBuilder shipBuilder;
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
        //Vector3 spawnStartPos = new Vector3(0, 0, -100);
        //Vector3 aiSpawnPos = new Vector3(0,0,100);
        int numShips = playerFleetData.gridIndex_metaData_table.Count;

        
        /////////////////////////////////////////////////

        TurnBasedCombatSystem.Instance.Init();

        if (numShips == 0)
        {
            Debug.LogWarning("Empty player fleet - spawning default fleet");
            playerStartSpawnPos.x -= gridDimensions.x * numShips / 2;
            foreach (string bpTemplateName in new List<string>() { "DefaultCorvette", "DefaultCorvette" })
            {
                TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.PlayerShip, BlueprintTemplates.GetBPTemplate(bpTemplateName), playerStartSpawnPos, Quaternion.identity);
                #if FULL_DEBUG
                if (unit == null)
                {
                    Debug.Log("shipbuilder returned null");
                }
                #endif
                TurnBasedCombatSystem.Instance.AddShip(unit);
                playerStartSpawnPos.x += gridDimensions.x;
            }
        }
        else
        {
            //tells the shipbuilder to build each ship in the fleet data
            foreach (var gridIndex_metaData in playerFleetData.gridIndex_metaData_table)
            {
                int index = gridIndex_metaData.Key;
                Vector2 gridPos = new Vector2(index % gridWidth, index / gridWidth);
                Vector3 spawnPos = new Vector3(gridPos.x * gridDimensions.x, 0.0f, gridPos.y * gridDimensions.y);
                Debug.Log("Index: " + index + " grid pos " + gridPos + "SpawnPos: " + playerStartSpawnPos + spawnPos);
                TurnBasedCombatSystem.Instance.AddShip(shipBuilder.BuildShip(ShipType.PlayerShip, gridIndex_metaData.Value.BlueprintName, playerStartSpawnPos + spawnPos, Quaternion.identity));
                //spawnStartPos.x += spawnSpacing;
            }
            //foreach (string blueprintName in playerFleetData.gridIndex_metaData_table.Values.Select(meta=>meta.BlueprintName))
            //{
            //    TurnBasedCombatSystem.Instance.AddShip(shipBuilder.BuildShip(ShipType.PlayerShip, blueprintName, spawnPos, Quaternion.identity));
            //    spawnPos.x += spawnSpacing;
            //}
        }
        if (pirateFleetData.currentFleet_BlueprintNames.Count == 0)
        {
            Debug.LogWarning("Empty enemy fleet - spawning default fleet");
            pirateFleetData.currentFleet_BlueprintNames = new List<string>() { "AI_Corvette", "AI_Frigate" };
        }

        foreach (string bpTemplateName in pirateFleetData.currentFleet_BlueprintNames)
        {
            TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.AI_Ship, BlueprintTemplates.GetBPTemplate(bpTemplateName), aiStartSpawnPos, Quaternion.identity);
            #if FULL_DEBUG
            if (unit == null)
            {
                Debug.Log("shipbuilder returned null");
            }
            #endif

            TurnBasedCombatSystem.Instance.AddShip(unit);
            aiStartSpawnPos.x -= gridDimensions.x;
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
