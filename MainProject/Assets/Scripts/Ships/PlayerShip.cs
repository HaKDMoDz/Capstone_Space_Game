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
using System.Linq;
using System;
#endregion Usings

/// <summary>
/// This class is the primary controller for the player ships. 
/// It handles logic for the various states that a playership can be in (The logic for the various states are separated into partial classes). 
/// Execute Turn is the primary loop for this class and handles state transitions.
/// </summary>
public partial class PlayerShip : TurnBasedUnit
{
    #region Fields
    public enum PlayerState { MovementMode, TacticalView, TargetingEnemy, ActivateWeapons}

    //member vars
    private PlayerState currentState;
    private bool shouldEndTurn;
    private bool shouldChangeState;
    //references
    private CombatSystemInterface combatInterface;
    private LineRenderer line;
    private Material lineMat;

    #endregion Fields
    #region Methods

    //Main Logic
    protected override void PreTurnActions()
    {
        Debug.Log("PreTurnActions");
        currentState = PlayerState.MovementMode;
        shouldEndTurn = false;
        shouldChangeState = false;
        combatInterface.ShowStatsPanel(true);
        combatInterface.UpdateStats(CurrentPower, MoveCost,false);
        combatInterface.SetPowerValid();
        combatInterface.ShowModeButtons(true);
        combatInterface.SetEndTurnEvent(()=>EndTurn(KeyCode.Return));
        InputManager.Instance.RegisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
        TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.MovementHowTo, true);
    }
    /// <summary>
    /// Execution for the player ship loops in this coroutine. It handles state transitions.
    /// </summary>
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        PreTurnActions();
        while(!ShouldTurnEnd())
        {
            Debug.LogWarning("Current State: " + currentState);
            yield return StartCoroutine(currentState.ToString());
            shouldChangeState = false;
        }
        PostTurnActions();
    }
    protected override void PostTurnActions()
    {
        Debug.Log("PostTurnActions");
        combatInterface.ShowStatsPanel(false);
        HideLineRenderer();
        //combatInterface.ShowModeButtons(false);
        InputManager.Instance.DeregisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
    }
    //Public Methods
    public void EndTurn(KeyCode key)
    {
        Debug.LogWarning("End Turn");
        TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.EndTurn, false);
        shouldEndTurn = true;
    }
    public override void Init(ShipBlueprint shipBP, ShipMove shipMove)
    {
        base.Init(shipBP, shipMove);
        combatInterface = CombatSystemInterface.Instance;
        spaceGround = SpaceGround.Instance;
        ConfigureShip();
    }
    //Private Methods
    /// <summary>
    /// returns true if the turn should end
    /// </summary>
    private bool ShouldTurnEnd()
    {
        return shouldEndTurn  //hit enter or end turn button
            || CurrentPower <= 0 
            || TurnBasedCombatSystem.Instance.ai_Ships.Count == 0; //no AI ships left
    }
    private void ChangeState(PlayerState nextState)
    {
        if(TutorialSystem.Instance)
        {
            TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.StartTacticalView);
        }
        currentState = nextState;
        Debug.Log("Change state to " + currentState);
        shouldChangeState = true;
    }
    //Helper
    private void DisplayLineRenderer(Vector3 targetPos, Color color)
    {
        line.enabled = true;
        lineMat.SetColor("_TintColor", color);
        line.SetVertexCount(2);
        line.SetPosition(0, trans.position + ComponentGridTrans.position);
        line.SetPosition(1, targetPos);
    }
    private void HideLineRenderer()
    {
        if(line) line.enabled = false;
    }
    private void ConfigureShip()
    {
        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/LineRenderer");
        GameObject lineObj = Instantiate(linePrefab) as GameObject;
        lineObj.transform.SetParent(trans, false);
        line = lineObj.GetComponent<LineRenderer>();
        lineMat = line.renderer.material;
        line.enabled = false;

        //setup targeting arcs
        GameObject targetingArcs = new GameObject("TargetingArcs");
        targetingArcTrans = targetingArcs.transform;
        targetingArcTrans.Translate(Vector3.up);
        //targetingArcTrans.RotateAroundXAxis(90.0f);
        targetingArcTrans.SetParent(trans, false);
        float yPos = 0.5f;
        foreach (Component_Weapon weapon in 
            components
            .Where(c => c is Component_Weapon)
            .Select(c => c.GetType())
            .Distinct()
            .Select((type)=>(Component_Weapon)components.Find(c=>c.GetType()==type))
            .OrderBy((weapon)=>weapon.range))
        {
            //Component_Weapon weapon = (Component_Weapon)components.First(c => c.GetType() == type);
            GameObject arcObj = new GameObject(weapon.componentName + " Arc");
            Transform arcTrans = arcObj.transform;
            arcTrans.SetParent(targetingArcTrans, false);
            arcTrans.SetPositionY(yPos);
            arcTrans.RotateAroundXAxis(90.0f);
            ArcMesh arc = arcObj.AddComponent<ArcMesh>();
            Material arcMat = new Material(PlayerShipConfig.ArcMat);
            arcMat.color = weapon.WeaponColour.WithAplha(PlayerShipConfig.ArcAlpha);
            arc.BuildArc(weapon.range, PlayerShipConfig.ArcAngle, PlayerShipConfig.ArcSegments, arcMat);
            yPos -= 0.5f;
        }
        ShowTargetingArc(false);
    }
    #endregion Methods
}
