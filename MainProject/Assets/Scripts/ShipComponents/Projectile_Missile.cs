/*
  Projectile_Missile.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 18/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Projectile_Missile : MonoBehaviour 
{
    public delegate void CollisionEvent(GameObject other);
    public event CollisionEvent OnCollision = new CollisionEvent((GameObject) => { });
    
    private void OnTriggerEnter(Collider other)
    {
        OnCollision(other.gameObject);    
    }

}
