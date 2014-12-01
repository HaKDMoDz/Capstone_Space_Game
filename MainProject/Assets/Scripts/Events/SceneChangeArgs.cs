using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SceneChangeArgs
{
    private GameScene previousScene;
    public GameScene PreviousScene
    {
        get { return previousScene; }
    }

    private GameScene nextScene;
    public GameScene NextScene
    {
        get { return nextScene; }
    }

    public SceneChangeArgs(GameScene previousScene, GameScene nextScene)
    {
        this.previousScene = previousScene;
        this.nextScene = nextScene;
    }
}
