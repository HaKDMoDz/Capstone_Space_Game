using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData
{
    public GameScene prevScene;
    public GameScene nextScene;
    public GalaxyMapData galaxyMapData;
    public EconomyData economyData;
    public MissionData missionData;
    public AI_Data eridani_AI_Data;
    public AI_Data kTaeran_AI_Data;
    public AI_Data pirates_AI_Data;
    public PlayerFleetData playerFleetData;

    public GameData()
    {
        Init();
    }

    public GameData(GameScene prevScene, GameScene nextScene)
    {
        this.prevScene = prevScene;
        this.nextScene = nextScene;
        Init();
    }

    private void Init()
    {
        playerFleetData = new PlayerFleetData();
        pirates_AI_Data = new AI_Data();
        galaxyMapData = new GalaxyMapData();
    }

    /// <summary>
    /// Generates a serializable version of the gameData
    /// </summary>
    /// <param name="sz_gameData">
    /// gets populated with a seriazable version of the gameData
    /// </param>
    public void Serialize(ref SerializedGameData sz_gameData)
    {
        sz_gameData.prevScene = prevScene;
        sz_gameData.nextScene = nextScene;

        playerFleetData.Serialize(ref sz_gameData.sz_playerFleetData);

        pirates_AI_Data.Serialize(ref sz_gameData.sz_pirates_AI_Data);
        galaxyMapData.Serialize(ref sz_gameData.sz_galaxyMapData);
    }

}
