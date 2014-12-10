using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SceneChangeArgs
{
    public GameScene PreviousScene { get; private set; }
    public GameScene NextScene { get; private set; }

    public SceneChangeArgs(GameScene previousScene, GameScene nextScene)
    {
        this.PreviousScene = previousScene;
        this.NextScene = nextScene;
    }
}
