/*
  TimedAction.cs
  Mission: Invasion
  Created by Rohun Banerji on Dec 3/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimedAction : MonoBehaviour 
{
    public IEnumerator SetTimedAction(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        //Debug.Log(gameObject.name + " calling action");
        action();
    }
}
