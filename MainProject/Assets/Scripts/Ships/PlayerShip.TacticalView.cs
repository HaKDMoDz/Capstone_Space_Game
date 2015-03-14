/*
  PlayerShip.TacticalView.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerShip : TurnBasedUnit 
{

    private IEnumerator PreTacticalView()
    {
        Debug.Log("PreTacticalView");
        yield return null;
    }
    private IEnumerator TacticalView()
    {
        yield return StartCoroutine(PreTacticalView());
        Debug.Log("TacticalView");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        currentState = PlayerState.TargetingMode;
        yield return StartCoroutine(PostTacticalView());
    }
    private IEnumerator PostTacticalView()
    {
        Debug.Log("Post TacticalView");
        yield return null;
    }
}
