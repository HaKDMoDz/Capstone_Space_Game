using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI_Attack : MonoBehaviour 
{
    /// <summary>
    /// This method deals with the actual attack
    /// </summary>
    /// <param name="_target">the target of the attack. 
    /// note: it takes a unit... it can be used for 
    /// friendly fire</param>
    /// <returns>null</returns>
    public IEnumerator Attack(TurnBasedUnit _target, float _damageAmount, List<ShipComponent> components)
    {
        Debug.Log("Unit: " + _target + "takes: " + _damageAmount);
        //StartCoroutine(_target.TakeDamage(_damageAmount));

        foreach (Component_Weapon weapon in components.Where(c => c is Component_Weapon))
        {
            Debug.Log("activate AI weapon");

            yield return StartCoroutine(
            weapon.Fire(_target.transform,
                () =>
                {
                }));
        }

        yield return null;
    }
}