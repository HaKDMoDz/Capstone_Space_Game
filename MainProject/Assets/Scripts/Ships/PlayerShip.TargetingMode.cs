/*
  PlayerShip.TargetingMode.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerShip : TurnBasedUnit 
{
    private IEnumerator PreTargetingMode()
    {
        Debug.Log("PreTargetingMode");
        yield return null;
    }
    private IEnumerator TargetingMode()
    {
        yield return StartCoroutine(PreTargetingMode());
        Debug.Log("TargetingMode");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        currentState = PlayerState.ActivateWeapons;
        yield return StartCoroutine(PostTargetingMode());
    }
    private IEnumerator PostTargetingMode()
    {
        Debug.Log("Post TargetingMode");
        yield return null;
    }

}
