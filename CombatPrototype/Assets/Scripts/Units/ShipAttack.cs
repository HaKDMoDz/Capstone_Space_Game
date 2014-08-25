using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ShipAttack : MonoBehaviour
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    Transform shootPoint;

    List<Comp_Weapon_Laser> lasers;
    List<Comp_Weapon_Missile> missiles;

    //cached components
    Transform trans;
    ShipBlueprint shipBlueprint;

    //local vars
    bool firingComplete=false;

    public void Init()
    {
        trans = transform;
        shipBlueprint = gameObject.GetSafeComponent<ShipBlueprint>();
        lasers = shipBlueprint.weapons.OfType<Comp_Weapon_Laser>().ToList();
        missiles = shipBlueprint.weapons.OfType<Comp_Weapon_Missile>().ToList();
    }

    

    public IEnumerator FireLasers(Vector3 aimPos)
    {

        foreach (Comp_Weapon_Laser laser in lasers)
        {
            laser.Fire(shootPoint);
        }

        while (!firingComplete)
        {
            Debug.LogError("call back");
            yield return null;
        }


        //Vector3 shootDir = aimPos - trans.position;
        //trans.LookAt(aimPos);
        //shootDir.Normalize();
        //for (int i = 0; i < 6; i++)
        //{
        //    GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
        //    projectile.rigidbody.AddForce(shootDir * 7000f);
        //    yield return new WaitForSeconds(0.1f);
        //}
        yield return new WaitForSeconds(.75f);
        
    }
    public IEnumerator FireMissiles(Vector3 aimPos)
    {
        Vector3 shootDir = aimPos - trans.position;
        trans.LookAt(aimPos);
        shootDir.Normalize();
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation) as GameObject;
        projectile.rigidbody.AddForce(shootDir * 5000f);
        yield return new WaitForSeconds(1f);
    }
}
