using UnityEngine;
using System.Collections;

public class Comp_Weapon_Laser : Component_Weapon 
{


    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    float shootForce = 7000f;

    //public IEnumerator Fire(Transform shootPoint)
    //{
    //    //return base.Fire();
    //    GameObject laserClone = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
    //    laserClone.rigidbody.AddForce(shootPoint.forward * shootForce);
    //    yield return null;
    //}
    public void Fire(Transform shootPoint)
    {
        //return base.Fire();
        GameObject laserClone = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
        laserClone.rigidbody.AddForce(shootPoint.forward * shootForce);
        laserClone.GetComponent<Projectile_Laser>().ProjectileHitEvent += Comp_Weapon_Laser_ProjectileHitEvent;
    }

    void Comp_Weapon_Laser_ProjectileHitEvent()
    {
        Debug.Log("proj hit");
    }
    
}
