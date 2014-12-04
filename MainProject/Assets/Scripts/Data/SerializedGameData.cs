using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SerializedGameData
{
    public GameScene currentScene;

    public SerializedGameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }
}
