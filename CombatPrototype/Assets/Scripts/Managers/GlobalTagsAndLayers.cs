using UnityEngine;
using System.Collections;
using System;

public class GlobalTagsAndLayers : SingletonComponent<GlobalTagsAndLayers> 
{

    public Layers layers;
    public Tags tags;
}
[Serializable]
public class Layers
{
    public int groundLayer=8;
    public int playerShipLayer=9;
    public int componentsLayer=10;
}
[Serializable]
public class Tags
{
    public string enemyShipTag;
}
