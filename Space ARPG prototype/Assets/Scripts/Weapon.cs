using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour 
{

    public GameObject projectile;
    public Transform shootpoint;

    public float reloadTime;
    public float range;
    public float projectileSpeed;

    protected bool canFire=true;

    

    public virtual void Fire()
    {

    }

}
