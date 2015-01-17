using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TagsAndLayers : ScriptableObject
{
    #region Layers
    public int playerShipLayer = 8;
    public int componentSlotLayer = 9;
    public int componentsLayer = 10;
    public int enemyShipLayer = 11;

    public static int PlayerShipLayer;
    public static int ComponentSlotLayer;
    public static int ComponentsLayer ;
    public static int EnemyShipLayer ;

    #endregion Layers
    
    #region Tags
    public string enemyShipTag = "EnemyShip";

    public static string EnemyShipTag ;
    #endregion Tags

    private void OnEnable()
    {
        PlayerShipLayer = playerShipLayer;
        ComponentSlotLayer = componentSlotLayer;
        ComponentsLayer = componentsLayer;
        EnemyShipLayer = enemyShipLayer;

        EnemyShipTag = enemyShipTag;
    }
}
