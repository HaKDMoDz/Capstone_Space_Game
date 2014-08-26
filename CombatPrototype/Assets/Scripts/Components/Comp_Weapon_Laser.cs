using UnityEngine;
using System.Collections;
using System;

public class Comp_Weapon_Laser : Component_Weapon 
{


    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    float projectileSpeed = 70f;

    
    public void Fire(Transform shootPoint,Transform target, Action OnHit )
    {
        GameObject laserClone = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
        //laserClone.rigidbody.AddForce(shootPoint.forward * shootForce);
        
        float timeToImpact = Vector3.Distance(target.position, shootPoint.position) / projectileSpeed;
        StartCoroutine(laserClone.GetComponent<Projectile_Laser>().MoveProjectile(target.position,timeToImpact,OnHit));

    }

    
}
