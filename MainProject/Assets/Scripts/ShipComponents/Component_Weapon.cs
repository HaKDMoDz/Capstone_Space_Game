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


    public virtual IEnumerator Fire(Transform target, Action OnActivationComplete)
    {
        Debug.Log("Weapon fire");
        yield return null;
    }
}
