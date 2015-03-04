using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour 
{
#if FULL_DEBUG
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(TurnBasedCombatSystem.Instance.ai_Ships[0].TakeDamage(150.0f));
            
        }
    }
#endif

}
