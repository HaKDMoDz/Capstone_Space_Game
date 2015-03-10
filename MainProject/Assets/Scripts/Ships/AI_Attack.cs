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
        Debug.Log("Unit: " + _target + "takes: " + _damageAmount);
        //StartCoroutine(_target.TakeDamage(_damageAmount));
        //AI_Ship has the power stat
        //while (GetComponent<AI_Ship>().CurrentPower > GetComponent<AI_Ship>().Components.Where(c => c is Component_Weapon).Aggregate((curr, next) => curr.PowerDrain < next.PowerDrain ? curr : next).PowerDrain)
       //while (GetComponent<AI_Ship>().CurrentPower > 50)
        bool keepFiring = true;
        while (keepFiring)
        {
            foreach (Component_Weapon weapon in components.Where(c => c is Component_Weapon))
            {
                Debug.Log("activate AI weapon");
                //yield return StartCoroutine(weapon.Fire(_target.transform, () => { }));
                Debug.Log("comp hp: " + _target.CompHP + " power vs drain: " + GetComponent<AI_Ship>().CurrentPower + "/" + weapon.PowerDrain + "hullHP: " + _target.ParentShip.HullHP);
                if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0)
                {
                    GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                    Debug.Log("RemainingPower: " + GetComponent<AI_Ship>().CurrentPower);
                    yield return StartCoroutine(weapon.Fire(_target, () => { }));
                }
                else
                {
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    keepFiring = false;
                    break;
                }
            }
            yield return null;
        }

        yield return null;
    }
}