/*
  PlayerShip.TacticalView.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Tactical View Mode - used for selecting enemy ship to target
/// </summary>
public partial class PlayerShip : TurnBasedUnit 
{
    private Transform targetingArcTrans;

    private IEnumerator PreTacticalView()
    {
        Debug.Log("PreTacticalView");
        
        combatInterface.ShowModeButtons(true);
        combatInterface.EnableMoveButton(true, () => ChangeState(PlayerState.MovementMode));
        InputManager.Instance.RegisterKeysDown(SwitchToMovementMode, KeyCode.Space, KeyCode.Escape);
        InputManager.Instance.OnMouseMoveEvent += OnMouseMove;
        ShowTargetingArc(true);
        Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
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
#if FULL_DEBUG
        int numAIShips = aiShips.Count;
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
        ShowTargetingArc(false);
        Camera.main.transparencySortMode = TransparencySortMode.Default;
        SubscribeToAIShipMouseEvents(false);
        CameraDirector.Instance.SetFreeCamera(false);
        combatInterface.ShowModeButtons(false);
        InputManager.Instance.DeregisterKeysDown(SwitchToMovementMode, KeyCode.Space, KeyCode.Escape);
        combatInterface.SetCursorType(CursorType.Default);
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
                aiShip.ShowHPBars(false);
            }
        }
    }
    void aiShip_OnShipMouseExit(AI_Ship ship)
    {
        combatInterface.SetCursorType(CursorType.Default);
        ship.ShowHPBars(false);
    }
    void aiShip_OnShipMouseEnter(AI_Ship ship)
    {
        if(AIShipIsInMaxRange(ship))
        {
            combatInterface.SetCursorType(CursorType.Attack);
        }
        else
        {
            combatInterface.SetCursorType(CursorType.Invalid);
        }
        ship.ShowHPBars(true);
    }
    void aiShip_OnShipClick(AI_Ship ship)
    {
        if (AIShipIsInMaxRange(ship))
        {
            Debug.Log("on Ship click " + ship);
            TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ClickEnemyToEngage, false);
            combatInterface.SetCursorType(CursorType.Default);
            ship.ShowHPBars(false);
            targetShip = ship;
            ChangeState(PlayerState.TargetingEnemy);
        }
    }
    bool AIShipIsInMaxRange(AI_Ship ship)
    {
        float maxWeaponRange = components
            .Where(comp => comp is Component_Weapon)
            .Select(comp => (Component_Weapon)comp)
            .Max(weapon => weapon.range);
        float distance = Vector3.Distance(ship.transform.position, trans.position);
        return distance <= maxWeaponRange;
    }
    void OnMouseMove(Vector2 direction)
    {
        MouseOverSpaceGround();
        targetingArcTrans.LookAtWithSameY(mousePosOnGround);
    }
    private void SwitchToMovementMode(KeyCode key)
    {
        ChangeState(PlayerState.MovementMode);
    }
    private void ShowTargetingArc(bool show)
    {
        targetingArcTrans.gameObject.SetActive(show);               
    }
}
