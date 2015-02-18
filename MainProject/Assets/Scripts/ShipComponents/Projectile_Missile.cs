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
