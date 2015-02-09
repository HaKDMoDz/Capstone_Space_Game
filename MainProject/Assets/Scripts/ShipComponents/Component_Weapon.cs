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
        if(targetShip.ShieldStrength>=damage)
        {
            yield return StartCoroutine(targetShip.TakeDamage(damage));
        }
        else
        {
            float remainingDamage = damage-targetShip.ShieldStrength;
            float componentDamage = remainingDamage * (1.0f - hullDamagePercent / 100.0f);
            float hullDamage = remainingDamage * hullDamagePercent / 100.0f;
            yield return StartCoroutine(targetComp.TakeDamage(componentDamage));
            yield return StartCoroutine(targetComp.ParentShip.TakeDamage(hullDamage));
        }

        
        
        //if comp is armour
        //  compDmg+=compDmg*armourMod%/100
        //if(targetComp is Comp_Def_Shield)
        //{
        //    componentDamage += componentDamage * shieldDmgModifier / 100.0f;
        //}

        
    }
}
