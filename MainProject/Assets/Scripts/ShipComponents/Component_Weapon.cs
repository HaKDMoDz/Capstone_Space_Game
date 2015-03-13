/*
  Component_Weapon.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 22/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Component_Weapon : ShipComponent
{
    public float damage;
    public float range;
    public float hullDamagePercent;
    public float armourDmgModifier;
    public float shieldDmgModifier;

    [SerializeField]
    protected Transform shootPoint;
    [SerializeField]
    private Color weaponColour;
    public Color WeaponColour
    {
        get { return weaponColour; }
    }

    protected Transform targetTrans;

    /// <summary>
    /// Fires the weapon at the specified component and raises the OnActivationComplete callback once the animation is complete.
    /// </summary>
    /// <param name="targetComp"></param>
    /// <param name="OnActivationComplete"></param>
    /// <returns></returns>
    public virtual IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        Debug.Log("Weapon fire");
        yield return null;
    }

    /// /////////////////////////
    /// temp overloading for AI
    /// //////////////////////////
    public virtual IEnumerator Fire(Transform target, Action OnActivationComplete)
    {
        Debug.Log("Weapon fire");
        yield return null;
    }

    /// <summary>
    /// Calculates the damage to be done based on the target's shielding, and the weapon's stats for hull penetration, etc. Calls the TakeDamage coroutines on the target component and target ship.
    /// </summary>
    /// <param name="targetComp"></param>
    /// <returns></returns>
    protected IEnumerator DoDamage(ShipComponent targetComp)
    {
        TurnBasedUnit targetShip = targetComp.ParentShip;
        if (!targetShip)
        {
            #if FULL_DEBUG
            Debug.LogError("Target ship is dead");
            #endif
        }
        else
        {
            if (targetShip.ShieldStrength >= damage)
            {
                ApplyShieldDamageEffect(targetComp.ParentShip);
                yield return StartCoroutine(targetShip.TakeDamage(damage));
            }
            else
            {
                Debug.Log("Killing shields " + targetShip.ShieldStrength);
                yield return StartCoroutine(targetShip.TakeDamage(targetShip.ShieldStrength));
                float remainingDamage = damage - targetShip.ShieldStrength;
                float componentDamage = remainingDamage * (1.0f - hullDamagePercent / 100.0f);
                float hullDamage = remainingDamage * hullDamagePercent / 100.0f;
                yield return StartCoroutine(targetComp.TakeDamage(componentDamage));
                yield return StartCoroutine(targetComp.ParentShip.TakeDamage(hullDamage));
            }
        }
        //if comp is armour
        //  compDmg+=compDmg*armourMod%/100
        //if(targetComp is Comp_Def_Shield)
        //{
        //    componentDamage += componentDamage * shieldDmgModifier / 100.0f;
        //}
    }

    protected void ApplyShieldDamageEffect(TurnBasedUnit ship)
    {
        Vector3 hitPoint=-Vector3.zero;
        Ray ray = new Ray(shootPoint.position, ship.transform.position - shootPoint.position);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1<<TagsAndLayers.ShipShieldLayer))
        {
            hitPoint = hit.point;
            ship.PlayShieldEffect(hitPoint);
        }
        #if FULL_DEBUG
        else
        {
            Debug.LogError("Weapon did not hit shields but was expected to");
        }
        #endif
    }
}
