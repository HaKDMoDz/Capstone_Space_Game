/*
  TagsAndLayers.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 16/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

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
    [SerializeField]
    private int shipShieldLayer = 16;
    


    public static int UI_Layer { get; private set; }
    public static int PlayerShipLayer {get; private set;}
    public static int ComponentSlotLayer {get; private set;}
    public static int SpaceSceneLayer { get; private set; }
    public static int SpaceGroundLayer { get; private set; }
    public static int ComponentsLayer { get; private set; }
    public static int AI_ShipLayer { get; private set; }
    public static int ShipShieldLayer { get; private set; }

    [SerializeField]
    private string motherShipTag = "MotherShip";
    [SerializeField]
    private string solarSystemTag = "SolarSystem";
    [SerializeField]
    private string planetTag = "Planet";
    [SerializeField]
    private string dropDownButtonTag = "DropDownButton";

    public static string MotherShipTag { get; private set; }
    public static string SolarSystemTag { get; private set; }
    public static string PlanetTag { get; private set; }
    public static string DropDownButtonTag { get; private set; }

    private void OnEnable()
    {
        UI_Layer = uiLayer;
        PlayerShipLayer = playerShipLayer;
        ComponentSlotLayer = componentSlotLayer;
        SpaceSceneLayer=spaceSceneLayer;
        SpaceGroundLayer = spaceGroundLayer;
        ComponentsLayer = componentsLayer;
        AI_ShipLayer = ai_ShipLayer;
        ShipShieldLayer = shipShieldLayer;

        MotherShipTag = motherShipTag;
        SolarSystemTag = solarSystemTag;
        PlanetTag = planetTag;
        DropDownButtonTag = dropDownButtonTag;
    }
}
