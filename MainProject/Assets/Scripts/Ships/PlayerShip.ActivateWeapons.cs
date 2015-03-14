/*
  PlayerShip.ActivateWeapons.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class PlayerShip : TurnBasedUnit 
{
    private IEnumerator PreActivateWeapons()
    {
        Debug.Log("PreActivateWeapons");
        yield return null;
    }
    private IEnumerator ActivateWeapons()
    {
        yield return StartCoroutine(PreActivateWeapons());
        Debug.Log("ActivateWeapons");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        currentState = PlayerState.MovementMode;
        yield return StartCoroutine(PostActivateWeapons());
    }
    private IEnumerator PostActivateWeapons()
    {
        Debug.Log("PostActivateWeapons");
        yield return null;
    }
}
