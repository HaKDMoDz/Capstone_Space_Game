using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData 
{
    public GameScene currentScene;

    public GameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }

    //public SerializedGameData Serialized()
    //{
    //    sz_gameData = new SerializedGameData(gameData.currentScene);
    //}
}
