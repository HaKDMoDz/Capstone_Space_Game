using UnityEngine;
using System.Collections;
using System;

public class Comp_Weapon_Laser : Component_Weapon 
{


    [SerializeField]
    GameObject projectilePrefab;
    //[SerializeField]
    //float projectileSpeed = 70f;

    //public override void Activate(Action OnComplete)
    //{
    //    base.Activate(OnComplete);
    //    GameObject laserClone = Instantiate(projectilePrefab, transform.position, transform.rotation) as GameObject;
    //    StartCoroutine(laserClone.GetComponent<Projectile_Laser>().MoveProjectile(transform.forward * 100f, 2f, OnComplete));
    //}

    public override void Fire(Transform target, Action OnHit )
    {
        //GameObject laserClone = Instantiate(projectilePrefab, transform.position, transform.rotation) as GameObject;
        ////laserClone.rigidbody.AddForce(shootPoint.forward * shootForce);

        //float timeToImpact = Vector3.Distance(target.position, transform.position) / projectileSpeed;
        //StartCoroutine(laserClone.GetComponent<Projectile_Laser>().MoveProjectile(target.position,timeToImpact,OnHit));

    }

    
}
