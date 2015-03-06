/*
  TimedDestroy.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 18/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimedDestroy : MonoBehaviour 
{
    [SerializeField]
    private float timer = 1.0f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

}
