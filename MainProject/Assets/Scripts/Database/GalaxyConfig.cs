/*
  GalaxyConfig.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 31/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyConfig : ScriptableObject 
{

    [SerializeField]
    private float systemAnimationPeriod = 5.0f;

    public static float SystemAnimationPeriod;

    private void OnEnable()
    {
        SystemAnimationPeriod = systemAnimationPeriod;
    }
	
}
