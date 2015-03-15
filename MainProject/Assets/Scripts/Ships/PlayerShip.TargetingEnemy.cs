/*
  PlayerShip.TargetingMode.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
#endregion Usings
/// <summary>
/// Targeting Enemy
/// </summary>
public partial class PlayerShip : TurnBasedUnit 
{
    private AI_Ship targetShip = null;
    private ShipComponent targetComponent = null;
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();
    private float totalActivationCost = 0.0f;
    private bool allowingEnemyTargeting = false;

    private IEnumerator PreTargetingEnemy()
    {
        #if FULL_DEBUG
        if(!targetShip) Debug.LogError("No target ship");
        Debug.Log("PreTargetingMode");
        #endif
        Transform aiTrans = targetShip.transform;
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTrans, GlobalVars.CameraAimAtPeriod));
        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowComponentSelectionPanel(true);
        combatInterface.ShowComponentHotkeyButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        ShowTargetingPanel(true);
        combatInterface.ShowModeButtons(true);
        combatInterface.EnableMoveButton(true, () => ChangeState(PlayerState.MovementMode));
        combatInterface.EnableTacticalButton(true, () => ChangeState(PlayerState.TacticalView));
        InputManager.Instance.RegisterKeysDown(SwitchToTacticalMode, KeyCode.Escape);
        targetComponent = null;
        trans.LookAt(aiTrans);
    }
    private IEnumerator TargetingEnemy()
    {
        yield return StartCoroutine(PreTargetingEnemy());
        Debug.Log("TargetingMode");
        while (!shouldChangeState && !ShouldTurnEnd())
        {
            yield return null;
        }
        yield return StartCoroutine(PostTargetingEnemy());
    }
    private IEnumerator PostTargetingEnemy()
    {
        Debug.Log("Post TargetingMode");
        combatInterface.ShowComponentSelectionPanel(false);
        ShowTargetingPanel(false);
        AllowEnemyTargeting(false);
        InputManager.Instance.DeregisterKeysDown(SwitchToTacticalMode, KeyCode.Escape);
        combatInterface.ShowModeButtons(false);
        HideLineRenderer();
        if (targetComponent) targetComponent.Selected = false;
        if(currentState!=PlayerState.ActivateWeapons)
        {
            UnSelectComponents();
            targetComponent = null;
        }
        yield return null;
    }
    private void SelectAllComponents(Type compType)
    {
        #if FULL_DEBUG
        Debug.Log("Activating all " + compType.ToString());
        #endif
        UnSelectComponents();
        //next tutorial hotkeys
        foreach (ShipComponent component in components.Where(c => c.GetType() == compType))
        {
            SelectComponent(component, true);
        }
    }
    private void SelectComponent(ShipComponent component, bool select)
    {
        if(select)
        {
            //selected comp is not the same component type as selected - restart selection
            if(selectedComponents.Count>0 && component.GetType()!=selectedComponents[0].GetType())
            {
                UnSelectComponents();
            }
            //not enough power - return without selecting
            if((CurrentPower - totalActivationCost - component.ActivationCost) < 0)
            {
                #if FULL_DEBUG
                Debug.LogWarning("Not enough power");
                #endif
                combatInterface.SetPowerValid(false);
                return;
            }
            //checks passed - select comp
            selectedComponents.Add(component);
            //tutorial next comp selection
            if (!allowingEnemyTargeting) AllowEnemyTargeting(true); ;
            totalActivationCost += component.ActivationCost;
        }//Select true
        else
        {
            #if FULL_DEBUG
            if(!selectedComponents.Contains(component))
            {
                Debug.LogError("Selected components doesn't contain " + component.componentName);
            }
            #endif
            selectedComponents.Remove(component);
            if (selectedComponents.Count == 0) AllowEnemyTargeting(false);
            totalActivationCost -= component.ActivationCost;
        }
        component.Selected = select;
        combatInterface.UpdateStats(CurrentPower - totalActivationCost, MoveCost);
    }//SelectComponent
    private void UnSelectComponents()
    {
        for (int i = selectedComponents.Count-1; i >= 0; i--)
        {
            SelectComponent(selectedComponents[i], false);
        }
    }
    private void OnComponentClick(ShipComponent component)
    {
        SelectComponent(component, !selectedComponents.Contains(component));
    }
    private void AllowEnemyTargeting(bool allow)
    {
        allowingEnemyTargeting = allow;
        if (!allow) HideLineRenderer();
        SubscribeTargetShipComponentEvents(allow);
    }
    private void SubscribeTargetShipComponentEvents(bool subscribe)
    {
        #if FULL_DEBUG
        if (!targetShip)
        {
            Debug.LogWarning("Target ship is null");
            return;
        }
        #endif
        if (subscribe)
        {
            foreach (ShipComponent component in targetShip.Components)
            {
                component.OnComponentClicked += OnTargetShipComponentClicked;
                component.OnComponentMouseOver += OnTargetShipComponentMouseOver;
                component.OnComponentPointerExit += OnTargetShipComponentPointerExit;
            }
        }
        else
        {
            foreach (ShipComponent component in targetShip.Components)
            {
                component.OnComponentClicked -= OnTargetShipComponentClicked;
                component.OnComponentMouseOver -= OnTargetShipComponentMouseOver;
                component.OnComponentPointerExit -= OnTargetShipComponentPointerExit;
            }
        }
    }//SubscribeTargetShipComponentEvents
    void OnTargetShipComponentClicked(ShipComponent component)
    {
        if (targetComponent) targetComponent.Selected = false;
        targetComponent = GetFirstComponentInDirection(component);
        targetComponent.Selected = true;
        //attack confirmed
        ChangeState(PlayerState.ActivateWeapons);
    }
    void OnTargetShipComponentMouseOver(ShipComponent component)
    {
        if (targetComponent) targetComponent.Selected = false;
        targetComponent = GetFirstComponentInDirection(component);
        targetComponent.Selected = true;
        combatInterface.ShowToolTip(component.componentName, Input.mousePosition);
        DisplayLineRenderer(targetComponent.transform.position, Color.cyan);
    }
    void OnTargetShipComponentPointerExit(ShipComponent component)
    {
        if (targetComponent) targetComponent.Selected = false;
        if (component.Selected) component.Selected = false;
        combatInterface.HideTooltip();
    }
    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Vector3 componentGridPos = trans.position + ComponentGridTrans.position;
        Ray ray = new Ray(componentGridPos, component.transform.position - componentGridPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1 << TagsAndLayers.ComponentsLayer))
        {
            return hit.collider.GetComponent<ShipComponent>();
        }
        //no component blocking
        return component;
    }
    private void ShowTargetingPanel(bool show)
    {
        #if FULL_DEBUG
        if (!targetShip)
        {
            Debug.LogError("no target ship");
            return;
        }
        #endif
        if (!show)
        {
            HideLineRenderer();
            combatInterface.HideTooltip();
        }
        targetShip.ShowTargetingPanel(show, trans);
    }
}
