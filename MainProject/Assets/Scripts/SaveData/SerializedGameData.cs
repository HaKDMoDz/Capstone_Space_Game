using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SerializedGameData
{
    public GameScene currentScene;
    public SerializedGalaxyMapData sz_galaxyMapData;
    public SerializedEconomyData sz_economyData;
    public SerializedMissionData sz_missionData;
    public SerializedAI_Data sz_eridani_AI_Data;
    public SerializedAI_Data sz_kTaeran_AI_Data;
    public SerializedAI_Data sz_pirates_AI_Data;

    public SerializedGameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }

    public void DeSerialize(ref GameData gameData)
    {
        gameData.currentScene = currentScene;
    }
}
