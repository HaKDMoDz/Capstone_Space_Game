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
    private Vector3 playerStartSpawnPos = new Vector3(-150.0f, 0.0f, -250.0f);
    [SerializeField]
    private Vector3 aiStartSpawnPos = new Vector3(-150.0f, 0.0f, 250.0f);
    [SerializeField]
    private MothershipLaunchCutscene launchCutscene;
    //references
    private PlayerFleetData playerFleetData;
    private AI_Data pirateFleetData;
    private ShipBuilder shipBuilder;
    Dictionary<Transform, Vector3> ship_gridPos_table = new Dictionary<Transform, Vector3>();
    #endregion Fields

    #region Methods
    /// <summary>
    /// Setups up the combat scene: adds ships, background objects, etc., and then tells the turn based system to start combat
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupScene()
    {
#if FULL_DEBUG
        Debug.Log("Setup Combat Scene");
#endif
        //get the saved player fleet
        playerFleetData = GameController.Instance.GameData.playerFleetData;
        pirateFleetData = GameController.Instance.GameData.pirates_AI_Data;
        int numShips = playerFleetData.gridIndex_metaData_table.Count;
        TurnBasedCombatSystem.Instance.Init();
        if (numShips == 0)
        {
            Debug.LogWarning("Empty player fleet - spawning default fleet");
            playerStartSpawnPos.x -= gridDimensions.x * numShips / 2;
            foreach (string bpTemplateName in new List<string>() { "DefaultCorvette", "DefaultCorvette" })
            {
                TurnBasedUnit unit = shipBuilder.BuildShip(ShipType.PlayerShip, BlueprintTemplates.GetBPTemplate(bpTemplateName), playerStartSpawnPos, Quaternion.identity);
#if FULL_DEBUG
                if (!unit) Debug.LogError("shipbuilder returned null");
#endif
                TurnBasedCombatSystem.Instance.AddShip(unit);
                ship_gridPos_table.Add(unit.transform, playerStartSpawnPos);
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
                TurnBasedUnit playerShip = shipBuilder.BuildShip(ShipType.PlayerShip, gridIndex_metaData.Value.BlueprintName, playerStartSpawnPos + spawnPos, Quaternion.identity);
#if FULL_DEBUG
                if (!playerShip) Debug.LogError("ShipBuilder return null");
#endif
                TurnBasedCombatSystem.Instance.AddShip(playerShip);
                //records ship grid positions for cutscene
                ship_gridPos_table.Add(playerShip.transform, playerStartSpawnPos + spawnPos);
            }
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
            if (!unit) Debug.LogError("shipbuilder returned null");
#endif
            TurnBasedCombatSystem.Instance.AddShip(unit);
            aiStartSpawnPos.x -= gridDimensions.x;
            unit.transform.RotateAroundYAxis(180.0f);
        }
        //combat start 
        //InputManager.Instance.RegisterKeysDown(SkipCutscene, KeyCode.Escape);
#if UNITY_EDITOR
        if (!GameObject.Find("CombatSystemTester"))
        {
            yield return StartCoroutine(launchCutscene.PlayCutscene(ship_gridPos_table));
            //InputManager.Instance.DeregisterKeysDown(SkipCutscene, KeyCode.Escape);
            yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
        }
#else
        yield return StartCoroutine(launchCutscene.PlayCutscene(ship_gridPos_table));
        //InputManager.Instance.DeregisterKeysDown(SkipCutscene, KeyCode.Escape);
        yield return StartCoroutine(TurnBasedCombatSystem.Instance.StartCombat());
#endif
    }//SetupScene

    //private void SkipCutscene(KeyCode key)
    //{
    //    launchCutscene.enabled = false;
    //    foreach (var ship_gridPos in ship_gridPos_table)
    //    {
    //        ship_gridPos.Key.position = ship_gridPos.Value;
    //    }
    //}
    #region UnityCallbacks

    private IEnumerator Start()
    {
        shipBuilder = new ShipBuilder();
        AudioManager.Instance.SetMainTrack(Sound.SciFiTheme);
        yield return StartCoroutine(SetupScene());
    }

    #endregion UnityCallbacks


    #endregion Methods
}
