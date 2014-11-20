using UnityEngine;
using System.Collections;
using System;

public class GlobalTagsAndLayers : Singleton<GlobalTagsAndLayers>
{
    public Layers layers;
    public Tags tags;

}
[Serializable]
public class Layers
{
    public int playerShipLayer = 8;
    public int componentTileLayer = 9;
    public int componentsLayer = 10;
    public int enemyShipLayer = 11;
}
[Serializable]
public class Tags
{
    public string enemyShipTag;
}
