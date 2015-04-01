/*
  PlayerShipConfig.cs
  Mission: Invasion
  Created by Rohun Banerji on March 13, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShipConfig : ScriptableObject
{
    [SerializeField]
    private float arcAngle = 30.0f;
    [SerializeField]
    private int arcSegments = 20;
    [SerializeField]
    private Material arcMat;
    [SerializeField]
    private float arcAlpha = 0.55f;
    [SerializeField]
    private Vector2 weaponActivationInterval = new Vector2(0.2f,0.4f);
    [SerializeField]
    private float movementRadius = 10.0f;

    public static float ArcAngle {get; private set;}
    public static int ArcSegments{get; private set;}
    public static Material ArcMat{get; private set;}
    public static float ArcAlpha{get; private set;}
    public static Vector2 WeaponActivationInterval{get; private set;}
    public static float MovementRadius {get; private set;}

    private void OnEnable()
    {
        ArcAngle = arcAngle;
        ArcSegments = arcSegments;
        ArcMat = arcMat;
        ArcAlpha = arcAlpha;
        WeaponActivationInterval = weaponActivationInterval;
        MovementRadius = movementRadius;
    }
}
