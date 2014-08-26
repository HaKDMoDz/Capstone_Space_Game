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
    public int groundLayer;
}
[Serializable]
public class Tags
{
    public string enemyShipTag;
}
