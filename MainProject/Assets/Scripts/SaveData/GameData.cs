using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData 
{
    public GameScene currentScene;
    public GalaxyMapData galaxyMapData;
    public EconomyData economyData;
    public MissionData missionData;
    public AI_Data eridani_AI_Data;
    public AI_Data kTaeran_AI_Data;
    public AI_Data pirates_AI_Data;

    public GameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }

    public SerializedGameData Serialized()
    {
        return new SerializedGameData(currentScene);
    }
    
}
