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
    SpaceGround spaceGround;

    //helper
    private bool receivedMoveCommand = false;
    private Vector3 mousePosOnGround;
    private float moveDistance;
    private float movePowerCost;

    #endregion Fields
    #region Methods
    private IEnumerator PreMovementMode()
    {
#if FULL_DEBUG
        if (!trans) Debug.LogError("Ship destroyed");
        Debug.Log("Pre Movement Mode");
#endif
        spaceGround.OnGroundClick += SpaceGroundClick;
        ShowMovementUI(true);
        combatInterface.ShowModeButtons(true);
        combatInterface.EnableTacticalButton(true, () => ChangeState(PlayerState.TacticalView));
        InputManager.Instance.RegisterKeysDown(SwitchToTacticalMode, KeyCode.Space);
        yield return null;
    }
    private IEnumerator MovementMode()
    {
        //yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
        yield return StartCoroutine(PreMovementMode());
        Debug.Log("Movement Mode");
        while(!shouldChangeState && !ShouldTurnEnd())
        {
            ShowMovementUI(true);
            MouseOverSpaceGround();
            if(receivedMoveCommand)
            {
                //hide movement ui
                ShowMovementUI(false);
                TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.MovementHowTo);
                CurrentPower -= movePowerCost;
                yield return StartCoroutine(shipMove.Move());
                receivedMoveCommand = false;
                ShowMovementUI(true);
                #if FULL_DEBUG
                Debug.Log(ShipBPMetaData.BlueprintName + "- Movement end");
                #endif
                
            }
            yield return null;
        }//while !change state and !end turn
        yield return StartCoroutine(PostMovementMode());
    }
    private IEnumerator PostMovementMode()
    {
        Debug.Log("Post Movement Mode");
        spaceGround.OnGroundClick -= SpaceGroundClick;
        ShowMovementUI(false);
        combatInterface.ShowModeButtons(false);
        InputManager.Instance.DeregisterKeysDown(SwitchToTacticalMode, KeyCode.Space);
        yield return null;
    }
    private void SwitchToTacticalMode(KeyCode key)
    {
        ChangeState(PlayerState.TacticalView);
    }
    //Helper
    private void ShowMovementUI(bool show)
    {
        if(show)
        {
            Color lineColour = movePowerCost > CurrentPower? Color.red: Color.cyan;
            DisplayLineRenderer(mousePosOnGround, lineColour);
            combatInterface.ShowMoveCostUI(mousePosOnGround, moveDistance, moveDistance * MoveCost);
        }
        else
        {
            combatInterface.HideMoveUI();
            HideLineRenderer();
        }
        //TODO: check collision
    }
    private void MouseOverSpaceGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1 << TagsAndLayers.SpaceGroundLayer))
        {
            mousePosOnGround = hit.point;
            moveDistance = Vector3.Distance(mousePosOnGround, trans.position);
            movePowerCost = Mathf.Round(moveDistance * MoveCost);
        }
    }
    #region InternalCallbacks
    void SpaceGroundClick(Vector3 worldPosition)
    {
        if (!receivedMoveCommand)
        {
            if (movePowerCost <= CurrentPower)
            {
                shipMove.destination = worldPosition;
                receivedMoveCommand = true;
            }
            else
            {
                combatInterface.SetPowerValid(false);

            }
        }
    }
    #endregion InternalCallbacks
    #endregion Methods
}
