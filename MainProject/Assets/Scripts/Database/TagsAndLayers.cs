using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TagsAndLayers : ScriptableObject
{
    [SerializeField]
    private int uiLayer = 5;
    [SerializeField]
    private int playerShipLayer = 8;
    [SerializeField]
    private int componentSlotLayer = 9;
    [SerializeField]
    private int spaceSceneLayer = 10;
    [SerializeField]
    private int spaceGroundLayer = 11;
    [SerializeField]
    private int componentsLayer = 12;
    [SerializeField]
    private int ai_ShipLayer = 13;


    public static int UI_Layer { get; private set; }
    public static int PlayerShipLayer {get; private set;}
    public static int ComponentSlotLayer {get; private set;}
    public static int SpaceSceneLayer { get; private set; }
    public static int SpaceGroundLayer { get; private set; }
    public static int ComponentsLayer { get; private set; }
    public static int AI_ShipLayer { get; private set; }

    [SerializeField]
    private string motherShipTag = "MotherShip";
    [SerializeField]
    private string solarSystemTag = "SolarSystem";

    public static string MotherShipTag { get; private set; }
    public static string SolarSystemTag { get; private set; }


    private void OnEnable()
    {
        UI_Layer = uiLayer;
        PlayerShipLayer = playerShipLayer;
        ComponentSlotLayer = componentSlotLayer;
        SpaceSceneLayer=spaceSceneLayer;
        SpaceGroundLayer = spaceGroundLayer;
        ComponentsLayer = componentsLayer;
        AI_ShipLayer = ai_ShipLayer;

        MotherShipTag = motherShipTag;
        SolarSystemTag = solarSystemTag;
    }
}
