/*
  PlayerShip.ActivateWeapons.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion Usings

public partial class PlayerShip : TurnBasedUnit
{
    private IEnumerator PreActivateWeapons()
    {
#if FULL_DEBUG
        if (!targetShip) Debug.LogError("No target ship");
        if (!targetComponent) Debug.LogError("No target component");
        if (selectedComponents == null || selectedComponents.Count == 0) Debug.LogError("no selected components");
        if (selectedComponents.Any(c => !(c is Component_Weapon))) Debug.LogError("Not weapon");
        Debug.Log("PreActivateWeapons");
#endif
        //tutorial hide click comp to fire
        targetShip.ShowHPBars(true);
        InputManager.Instance.DeregisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
        trans.LookAt(targetComponent.transform);
        yield return StartCoroutine(CameraDirector.Instance.ZoomInFromAbove(targetComponent.ParentShip.transform, GlobalVars.CameraAimAtPeriod));
        yield return null;
    }
    private IEnumerator ActivateWeapons()
    {
        int originalCamCulling = Camera.main.cullingMask;
        Camera.main.cullingMask = originalCamCulling | 1 << TagsAndLayers.ComponentsLayer | 1 << TagsAndLayers.ComponentSlotLayer;
        yield return StartCoroutine(PreActivateWeapons());
        //how many weapons to activate
        Component_Weapon[] selectedWeapons = selectedComponents.Cast<Component_Weapon>().ToArray();
        int numWeaponsToActivate = GetNumWeaponsToActivate(selectedWeapons[0]);
#if FULL_DEBUG
        Debug.Log("ActivateWeapons");
        Debug.Log("numWeaponsToActivate " + numWeaponsToActivate);
#endif
        float totalPowerUsed = numWeaponsToActivate * selectedWeapons[0].ActivationCost;
        CurrentPower -= totalPowerUsed;
        int weaponHitCounter = 0;
        for (int i = 0; i < numWeaponsToActivate; i++)
        {
            StartCoroutine(selectedWeapons[i].Fire(targetComponent,
                    () => { weaponHitCounter++; }));
            yield return new WaitForSeconds(Random.Range(PlayerShipConfig.WeaponActivationInterval.x, PlayerShipConfig.WeaponActivationInterval.y));
        }
        while(weaponHitCounter<numWeaponsToActivate)
        {
            yield return null;
        }
        Camera.main.cullingMask = originalCamCulling;
        yield return StartCoroutine(PostActivateWeapons());
    }
    private IEnumerator PostActivateWeapons()
    {
        Debug.Log("PostActivateWeapons");
        //UnSelectComponents();
        if (targetComponent)
        {
            targetComponent.Selected = false;
            targetComponent = null;
        }
        //Debug.Log(targetShip + " hp " + targetShip.HullHP);
        if (targetShip && targetShip.HullHP>0.0f)
        {
            targetShip.ShowHPBars(false);
            ChangeState(PlayerState.TargetingEnemy);
        }
        else
        {
            UnSelectComponents();
            ChangeState(PlayerState.MovementMode);
            yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
        }
        InputManager.Instance.RegisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
        TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.EndTurn, true);
        yield return null;
    }
    /// <summary>
    /// Returns the number of weapon activations required to kill the target component or ship - avoids over-firing
    /// </summary>
    /// <returns></returns>
    private int GetNumWeaponsToActivate(Component_Weapon weapon)
    {
        int numWeaponsToKillShields = Mathf.CeilToInt(targetShip.ShieldStrength / weapon.ShieldDamage);
#if FULL_DEBUG
        //Debug.LogWarning("Weapon activation calculation: ");
        //Debug.Log("Target shield: " + targetShip.ShieldStrength + " weapon shield dmg " + weapon.ShieldDamage + " num to kill shield " + numWeaponsToKillShields);
#endif
        if (numWeaponsToKillShields > selectedComponents.Count)
        {
            return selectedComponents.Count;
        }
        int numWpnsToKillComp = Mathf.CeilToInt(targetComponent.CompHP / weapon.ComponentDamage);
        int numWpnsToKillHull = Mathf.CeilToInt(targetShip.HullHP / weapon.HullDamage);
#if FULL_DEBUG
        //Debug.Log("Target comp HP: " + targetComponent.CompHP + " weapon comp dmg " + weapon.ComponentDamage + " num to kill comp " + numWpnsToKillComp);
        //Debug.Log("Target hull HP: " + targetShip.HullHP + " weapon hull dmg " + weapon.HullDamage + " num to kill hull " + numWpnsToKillHull);
#endif
        //num weapon activations is the minimum to kill target component or to kill hull
        int totalWpnActivations = numWeaponsToKillShields + (numWpnsToKillComp < numWpnsToKillHull ? numWpnsToKillComp : numWpnsToKillHull);
        if (totalWpnActivations > selectedComponents.Count)
        {
            return selectedComponents.Count;
        }
        return totalWpnActivations;
    }
}
