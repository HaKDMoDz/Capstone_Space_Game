using UnityEngine;
using System.Collections;

public class AI_Attack : MonoBehaviour 
{
    /// <summary>
    /// This method deals with the actual attack
    /// </summary>
    /// <param name="_target">the target of the attack. 
    /// note: it takes a unit... it can be used for 
    /// friendly fire</param>
    /// <returns>null</returns>
    public IEnumerator Attack(TurnBasedUnit _target, float _damageAmount)
    {
        _target.takeDamage(_damageAmount);
        yield return null;
    }
}