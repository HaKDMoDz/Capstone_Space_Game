/*
  PlayerShip.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion Usings

/// <summary>
/// This class is the primary controller for the player ships. 
/// It handles logic for the various states that a playership can be in (The logic for the various states are separated into partial classes). 
/// Execute Turn is the primary loop for this class and handles state transitions.
/// </summary>
public partial class PlayerShip : TurnBasedUnit
{
    #region Fields
    public enum PlayerState { MovementMode, TacticalView, TargetingMode, ActivateWeapons}

    //member vars
    private PlayerState currentState;
    private bool continueTurn;

    //references
    //CombatSystemInterface combatInterface;

    #endregion Fields
    #region Methods

    //Main Logic
    protected override void PreTurnActions()
    {
        Debug.Log("PreTurnActions");
        currentState = PlayerState.MovementMode;
        continueTurn = true;
        InputManager.Instance.RegisterKeysDown((key) => EndTurn(), KeyCode.KeypadEnter, KeyCode.Return);
    }
    /// <summary>
    /// Execution for the player ship loops in this coroutine. It handles state transitions.
    /// </summary>
    public override IEnumerator ExecuteTurn()
    {
        PreTurnActions();
        while(continueTurn)
        {
            yield return StartCoroutine(currentState.ToString());
        }
        PostTurnActions();
    }
    protected override void PostTurnActions()
    {
        Debug.Log("PostTurnActions");
        InputManager.Instance.DeregisterKeysDown((key) => EndTurn(), KeyCode.KeypadEnter, KeyCode.Return);
    }
    //Public Methods
    public void EndTurn()
    {
        continueTurn = false;
    }
    public override void Init(ShipBlueprint shipBP, ShipMove shipMove)
    {
        base.Init(shipBP, shipMove);
        //combatInterface = CombatSystemInterface.Instance;
        //spaceGround = SpaceGround.Instance;

    }
    //Private Methods
    //Helper
    private void ConfigureShip()
    {

    }
    #endregion Methods
}
