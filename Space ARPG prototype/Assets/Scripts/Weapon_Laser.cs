using UnityEngine;
using System.Collections;

public class Weapon_Laser : Weapon 
{

    public override void Fire()
    {
        if(canFire)
        {
            StartCoroutine(FireLasers());
            canFire = false;
        }

        base.Fire();
    }

    IEnumerator FireLasers()
    {
        GameObject laserClone = Instantiate(projectile, shootpoint.position, shootpoint.rotation) as GameObject;
        laserClone.rigidbody.velocity = shootpoint.forward * projectileSpeed;
        Destroy(laserClone, 3.0f);

        yield return new WaitForSeconds(reloadTime);
        canFire = true;
    }
}
