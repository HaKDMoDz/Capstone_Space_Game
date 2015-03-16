/*
  PlayerShip.TacticalView.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Tactical View Mode - used for selecting enemy ship to target
/// </summary>
public partial class PlayerShip : TurnBasedUnit 
{
    private IEnumerator PreTacticalView()
    {
        Debug.Log("PreTacticalView");
        
        combatInterface.ShowModeButtons(true);
        combatInterface.EnableMoveButton(true, () => ChangeState(PlayerState.MovementMode));
        InputManager.Instance.RegisterKeysDown(SwitchToMovementMode, KeyCode.Space, KeyCode.Escape);
        SubscribeToAIShipMouseEvents(true);
        CameraDirector.Instance.SetFreeCamera(true);
        yield return null;
    }
    private IEnumerator TacticalView()
    {
        //Setup ai ships and camera before pre-tactical view
        //get first ai
        int targetShipIndex = 0;
        List<AI_Ship> aiShips = TurnBasedCombatSystem.Instance.ai_Ships;
        int numAIShips = aiShips.Count;
        #if FULL_DEBUG
        if (numAIShips == 0)
        {
            Debug.LogError("No ai ships found");
        }
        #endif
        AI_Ship targetShip = aiShips[targetShipIndex];
        targetShipIndex = aiShips.IndexOf(targetShip);
        Transform aiTargetTrans = targetShip.transform;
        //overhead cam
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTargetTrans, GlobalVars.CameraAimAtPeriod));
        yield return StartCoroutine(PreTacticalView());
        Debug.Log("TacticalView");
        while (!shouldChangeState && !ShouldTurnEnd())
        {
            yield return null;
        }
        yield return StartCoroutine(PostTacticalView());
    }
    private IEnumerator PostTacticalView()
    {
        Debug.Log("Post TacticalView");
        SubscribeToAIShipMouseEvents(false);
        CameraDirector.Instance.SetFreeCamera(false);
        combatInterface.ShowModeButtons(false);
        InputManager.Instance.DeregisterKeysDown(SwitchToMovementMode, KeyCode.Space, KeyCode.Escape);
        if (currentState == PlayerState.MovementMode)
        {
            yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
        }
        yield return null;
    }
    private void SubscribeToAIShipMouseEvents(bool subscribe)
    {
        if (subscribe)
        {
            foreach (AI_Ship aiShip in TurnBasedCombatSystem.Instance.ai_Ships)
            {
                aiShip.OnShipClick += aiShip_OnShipClick;
                aiShip.OnShipMouseEnter += aiShip_OnShipMouseEnter;
                aiShip.OnShipMouseExit += aiShip_OnShipMouseExit;
            }
        }
        else
        {
            foreach (AI_Ship aiShip in TurnBasedCombatSystem.Instance.ai_Ships)
            {
                aiShip.OnShipClick -= aiShip_OnShipClick;
                aiShip.OnShipMouseEnter -= aiShip_OnShipMouseEnter;
                aiShip.OnShipMouseExit -= aiShip_OnShipMouseExit;
            }
        }
    }
    void aiShip_OnShipMouseExit(AI_Ship ship)
    {
        combatInterface.ShowAttackCursor(false);
        ship.ShowHPBars(false);
    }
    void aiShip_OnShipMouseEnter(AI_Ship ship)
    {
            combatInterface.ShowAttackCursor(true);
            ship.ShowHPBars(true);
    }
    void aiShip_OnShipClick(AI_Ship ship)
    {
        Debug.Log("on Ship click " + ship);
        combatInterface.ShowAttackCursor(false);
        ship.ShowHPBars(false);
        targetShip = ship;
        ChangeState(PlayerState.TargetingEnemy);
    }
    private void SwitchToMovementMode(KeyCode key)
    {
        ChangeState(PlayerState.MovementMode);
    }
}
