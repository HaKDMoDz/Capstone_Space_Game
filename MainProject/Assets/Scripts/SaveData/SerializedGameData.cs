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
    }
}
