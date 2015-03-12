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
    public IEnumerator Attack(ShipComponent _target, float _damageAmount, List<ShipComponent> components)
    {
        bool keepFiring = true;
        while (keepFiring)
        {
            foreach (Component_Weapon weapon in components.Where(c => c is Component_Weapon))
            {
                if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0)
                {
                    GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                    yield return StartCoroutine(weapon.Fire(_target, () => { }));
                }
                else
                {
                    if (GetComponent<AI_Ship>().CurrentPower <= 50)
                    {
                        keepFiring = false;
                    }
                    break;
                }
            }
            yield return null;
        }
        yield return null;
    }
}