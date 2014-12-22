using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Component_Weapon : ShipComponent 
{
    public float damage;
    public float range;
    public float hullDamagePercent;
	public float armourDmgModifier;
    public float shieldDmgModifier;

    public abstract void Fire();
}
