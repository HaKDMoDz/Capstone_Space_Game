using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SerializedGameData
{
    public GameScene currentScene;

    SerializedGameData(GameScene currentScene)
    {
        this.currentScene = currentScene;
    }
}
