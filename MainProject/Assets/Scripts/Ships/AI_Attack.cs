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
    public IEnumerator Attack(ShipComponent _target, List<ShipComponent> components)
    {
        Debug.LogWarning(_target + " targetted");
        GetComponent<AI_Ship>().CurrentPower = GetComponent<AI_Ship>().MaxPower;
        bool keepFiring = true;



        while (keepFiring)
        {
            if (TurnBasedCombatSystem.Instance.playerShips == null || TurnBasedCombatSystem.Instance.playerShips.Count() <= 0)
            {
                keepFiring = false;
                break;
            }

            if (_target.ParentShip.HullHP <= 0)
            {
                GetComponent<AI_Ship>().RetargetNewShip();
                GetComponent<AI_Ship>().RetargetNewComponent();
            }

            if (_target.ParentShip.ShieldStrength > 0)
            {
                foreach (Comp_Wpn_Laser weapon in components.Where(c => c is Comp_Wpn_Laser))
                {
                    if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0)
                    {
                        GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                        yield return StartCoroutine(weapon.Fire(_target, () => { }));
                        
                        if (_target.ParentShip.HullHP <= 0)
                        {
                            GetComponent<AI_Ship>().RetargetNewShip();
                            GetComponent<AI_Ship>().RetargetNewComponent();
                        }
                    }
                    else
                    {
                        {
                            keepFiring = false;
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (Component_Weapon weapon in components.Where(c => c is Comp_Wpn_Missile || c is Comp_Wpn_Railgun))
                {
                    if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0)
                    {
                        GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                        yield return StartCoroutine(weapon.Fire(_target, () => { }));
                        
                        if (_target.ParentShip.HullHP <= 0)
                        {
                            GetComponent<AI_Ship>().RetargetNewShip();
                            GetComponent<AI_Ship>().RetargetNewComponent();
                        }
                    }
                    else
                    {
                        {
                            keepFiring = false;
                        }
                        break;
                    }
                }
            }
            
            yield return null;
        }
        yield return null;
    }
}