using UnityEngine;
using System.Collections;

public class Component_Weapon : ShipComponent 
{

    public float damage;

    public virtual IEnumerator Fire()
    {
        yield return null;
    }

}
