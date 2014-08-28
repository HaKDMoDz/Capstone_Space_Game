using UnityEngine;
using System.Collections;

public class Component_Weapon : ShipComponent 
{

    public float damage;
    public float range;

    public virtual void Fire(Transform target, System.Action OnActivationComplete)
    {

    }

}
