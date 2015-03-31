/*
  Cheats.cs
  Mission: Invasion
  Created by Rohun Banerji on Mar 4/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cheats : MonoBehaviour 
{
#if FULL_DEBUG
    void Start()
    {
        InputManager.Instance.RegisterKeysDown(Damage, KeyCode.D, KeyCode.F);
        InputManager.Instance.RegisterKeysDown((key) => ChangeToGalaxyMap(), KeyCode.G);
    }
    private void Damage(KeyCode key)
    {
        if (key == KeyCode.D && Input.GetKey(KeyCode.LeftShift) 
            && TurnBasedCombatSystem.Instance.ai_Ships[0])
        {
            StartCoroutine(TurnBasedCombatSystem.Instance.ai_Ships[0].TakeDamage(150.0f));
        }
        if (key == KeyCode.F && Input.GetKey(KeyCode.LeftShift)
            && TurnBasedCombatSystem.Instance.ai_Ships[0])
        {
            StartCoroutine(TurnBasedCombatSystem.Instance.ai_Ships[0].TakeDamage(TurnBasedCombatSystem.Instance.ai_Ships[0].ShieldStrength));
        }
    }
    private void ChangeToGalaxyMap()
    {
        if(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
        {
            GameController.Instance.ChangeScene(GameScene.GalaxyMap);
        }
    }

#endif

}
