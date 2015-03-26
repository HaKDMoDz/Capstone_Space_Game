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
    Transform aiTrans = null;
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();
    private float totalActivationCost = 0.0f;
    private bool allowingEnemyTargeting = false;

    private IEnumerator PreTargetingEnemy()
    {
#if FULL_DEBUG
        if (!targetShip) Debug.LogError("No target ship");
        Debug.Log("PreTargetingMode");
#endif
        aiTrans = targetShip.transform;
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTrans, GlobalVars.CameraAimAtPeriod));
        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowComponentSelectionPanel(true);
        //combatInterface.ShowComponentHotkeyButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        combatInterface.ShowComponentHotkeyButtons(SelectAllComponents, 
            components.Where(comp=>comp.CanActivate)
            .ToDictionary(comp=>comp,comp=>WeaponCanHitEnemy((Component_Weapon)comp)));
        ShowTargetingPanel(true);
        combatInterface.ShowModeButtons(true);
        combatInterface.EnableMoveButton(true, () => ChangeState(PlayerState.MovementMode));
        combatInterface.EnableTacticalButton(true, () => ChangeState(PlayerState.TacticalView));
        InputManager.Instance.RegisterKeysDown(SwitchToTacticalMode, KeyCode.Escape);
        TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ComponentPanel, true);
        spaceGround.Display(false);
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
        spaceGround.Display(true);
        combatInterface.ShowComponentSelectionPanel(false);
        ShowTargetingPanel(false);
        AllowEnemyTargeting(false);
        InputManager.Instance.DeregisterKeysDown(SwitchToTacticalMode, KeyCode.Escape);
        combatInterface.ShowModeButtons(false);
        HideLineRenderer();
        combatInterface.SetCursorType(CursorType.Default);
        if (targetComponent) targetComponent.Selected = false;
        if (currentState != PlayerState.ActivateWeapons)
        {
            UnSelectComponents();
            targetComponent = null;
            if(currentState==PlayerState.MovementMode)
            {
                yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
            }
        }
        yield return null;
    }
    private void SelectAllComponents(Type compType)
    {
        Component_Weapon weaponToSelect = components.FirstOrDefault(c => c.GetType() == compType) as Component_Weapon;
#if FULL_DEBUG
        if(!weaponToSelect)  Debug.LogError("Not weapon found of type " + compType);
#endif
        if(!WeaponCanHitEnemy(weaponToSelect))
        {
            Debug.Log("Cannot hit enemy");
            return;
        }

#if FULL_DEBUG
        Debug.Log("Activating all " + compType.ToString());
#endif
        UnSelectComponents();
        //next tutorial hotkeys
        TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.Hotkeys);
        foreach (ShipComponent component in components.Where(c => c.GetType() == compType))
        {
            SelectComponent(component, true);
        }
    }
    private bool WeaponCanHitEnemy(Component_Weapon weapon)
    {
        return weapon.range >= Vector3.Distance(trans.position, aiTrans.position);
    }
    private void SelectComponent(ShipComponent component, bool select)
    {
        if (select)
        {
            //selected comp is not the same component type as selected - restart selection
            if (selectedComponents.Count > 0 && component.GetType() != selectedComponents[0].GetType())
            {
                UnSelectComponents();
            }
            //not enough power - return without selecting
            if ((CurrentPower - totalActivationCost - component.ActivationCost) < 0)
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
            if (!selectedComponents.Contains(component))
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
        for (int i = selectedComponents.Count - 1; i >= 0; i--)
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
        TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ClickOnCompToFire, false);
    }
    void OnTargetShipComponentMouseOver(ShipComponent component)
    {
        if (targetComponent) targetComponent.Selected = false;
        targetComponent = GetFirstComponentInDirection(component);
        targetComponent.Selected = true;
        combatInterface.ShowToolTip(component.componentName, Input.mousePosition);
        DisplayLineRenderer(targetComponent.transform.position, Color.cyan);
        combatInterface.SetCursorType(CursorType.Attack);
    }
    void OnTargetShipComponentPointerExit(ShipComponent component)
    {
        if (targetComponent) targetComponent.Selected = false;
        if (component.Selected) component.Selected = false;
        combatInterface.HideTooltip();
        HideLineRenderer();
        combatInterface.SetCursorType(CursorType.Default);
    }
    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Vector3 componentGridPos = trans.position + ComponentGridTrans.position;
        Vector3 targetCompPos = component.transform.position;
        Vector3 directionToTargetComp = targetCompPos - componentGridPos;
        //Ray ray = new Ray(componentGridPos, targetCompPos - componentGridPos);
        RaycastHit[] hits = Physics.RaycastAll(componentGridPos, directionToTargetComp, GlobalVars.RayCastRange);
#if FULL_DEBUG
        if (hits == null || hits.Length == 0) Debug.LogError("No raycast hits");
#endif
        List<ShipComponent> hitComponents = new List<ShipComponent>();
        foreach (RaycastHit hit in hits)
        {
            ShipComponent comp = hit.collider.GetComponent<ShipComponent>();
            if (comp && comp.ParentShip != this)
            {
                hitComponents.Add(comp);
            }
        }
#if FULL_DEBUG
        if (hitComponents == null || hitComponents.Count() == 0) Debug.LogError("No hit components");
#endif
        ShipComponent closestComp = hitComponents
            .Select(c => c.transform)
            .Aggregate((curr, next) =>
                Vector3.Distance(curr.position, componentGridPos)
                < Vector3.Distance(next.position, componentGridPos)
                ? curr : next)
            .GetComponent<ShipComponent>();
#if FULL_DEBUG
        if (!closestComp)
        {
            Debug.LogError("No component found");
        }
#endif
        return closestComp;
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
