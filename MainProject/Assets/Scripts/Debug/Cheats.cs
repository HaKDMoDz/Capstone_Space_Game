using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour 
{
#if FULL_DEBUG
    void Start()
    {
        InputManager.Instance.RegisterKeysDown((key) => Damage(), KeyCode.D);
    }
    void Damage()
    {
        if (Input.GetKey(KeyCode.LeftShift) 
            && TurnBasedCombatSystem.Instance.ai_Ships[0])
        {
            StartCoroutine(TurnBasedCombatSystem.Instance.ai_Ships[0].TakeDamage(150.0f));
        }
    }

#endif

}
