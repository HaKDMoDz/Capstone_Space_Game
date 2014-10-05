using UnityEngine;
using System.Collections;
using System;

public class Comp_Weapon_Missile : Component_Weapon 
{

    [SerializeField]
    GameObject projectilePrefab;
    //[SerializeField]
    //float projectileSpeed = 60f;

    public override void Fire(Transform target, Action OnHit)
    {
        //GameObject laserClone = Instantiate(projectilePrefab, transform.position, transform.rotation) as GameObject;
        ////laserClone.rigidbody.AddForce(shootPoint.forward * shootForce);

        //float timeToImpact = Vector3.Distance(target.position, transform.position) / projectileSpeed;
        //StartCoroutine(laserClone.GetComponent<Projectile_Missile>().MoveProjectile(target.position, timeToImpact, OnHit));

    }

}
