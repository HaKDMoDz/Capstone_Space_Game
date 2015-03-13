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

    public static float ArcAngle;
    public static int ArcSegments;
    public static Material ArcMat;
    public static float ArcAlpha;

    private void OnEnable()
    {
        ArcAngle = arcAngle;
        ArcSegments = arcSegments;
        ArcMat = arcMat;
        ArcAlpha= arcAlpha;
    }
}
