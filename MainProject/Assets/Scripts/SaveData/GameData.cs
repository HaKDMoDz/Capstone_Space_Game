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


    public GameData() { }

    public GameData(GameScene prevScene, GameScene nextScene)
    {
        this.prevScene = prevScene;
        this.nextScene = nextScene;
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
    }

}
