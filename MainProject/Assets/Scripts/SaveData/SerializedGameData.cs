/*
  SerializedGameData.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 7/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SerializedGameData
{
    public GameScene prevScene;
    public GameScene nextScene;
    public SerializedGalaxyMapData sz_galaxyMapData;
    public SerializedEconomyData sz_economyData;
    public SerializedMissionData sz_missionData;
    public SerializedAI_Data sz_eridani_AI_Data;
    public SerializedAI_Data sz_kTaeran_AI_Data;
    public SerializedAI_Data sz_pirates_AI_Data;
    public SerializedPlayerFleetData sz_playerFleetData;
    public SerializedTutorialData sz_tutorialData;

    public SerializedGameData() 
    {
        Init();
    }

    public SerializedGameData(GameScene prevScene, GameScene nextScene)
    {
        this.prevScene = prevScene;
        this.nextScene = nextScene;
        Init();
    }

    private void Init()
    {
        sz_playerFleetData = new SerializedPlayerFleetData();
        sz_pirates_AI_Data = new SerializedAI_Data();
        sz_galaxyMapData = new SerializedGalaxyMapData();
        sz_tutorialData = new SerializedTutorialData();
    }
    /// <summary>
    /// Generates a deSerialized version of the serializedGameData
    /// </summary>
    /// <param name="gameData">
    /// gets populated with a concrete version of the serializedGameData
    /// </param>
    public void DeSerialize(ref GameData gameData)
    {
        gameData.prevScene = prevScene;
        gameData.nextScene = nextScene;

        sz_playerFleetData.DeSerialize(ref gameData.playerFleetData);
        sz_pirates_AI_Data.DeSerialize(ref gameData.pirates_AI_Data);
        sz_galaxyMapData.DeSerialize(ref gameData.galaxyMapData);
        sz_tutorialData.DeSerialize(ref gameData.tutorialData);
    }
}
