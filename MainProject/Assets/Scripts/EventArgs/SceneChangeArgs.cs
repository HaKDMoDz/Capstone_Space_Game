/*
  SceneChangeArgs.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 8/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
