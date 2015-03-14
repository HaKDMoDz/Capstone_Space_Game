/*
  PlayerShip.MovementMode.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion Usings

public partial class PlayerShip : TurnBasedUnit
{
    #region Fields
    //References
    //SpaceGround spaceGround;
    #endregion Fields
    #region Methods
    private IEnumerator PreMovementMode()
    {
        Debug.Log("Pre Movement Mode");
        yield return null;
    }
    private IEnumerator MovementMode()
    {
        yield return StartCoroutine(PreMovementMode());
        Debug.Log("Movement Mode");
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        currentState = PlayerState.TacticalView;
        yield return StartCoroutine(PostMovementMode());
    }
    private IEnumerator PostMovementMode()
    {
        Debug.Log("Post Movement Mode");
        yield return null;
    }
    #endregion Methods
}
