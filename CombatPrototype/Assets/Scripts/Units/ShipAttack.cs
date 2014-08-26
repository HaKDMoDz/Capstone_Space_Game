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

    

    public IEnumerator Fire(Vector3 aimPos)
    {
        firingComplete = false;
        
        trans.LookAt(aimPos);
        Transform selectedTarget = GameObject.FindObjectOfType<AIShip>().transform;

        for (int i = 0; i < lasers.Count; i++)
        {
            lasers[i].Fire(shootPoint,selectedTarget, Hit);
            yield return new WaitForSeconds(0.1f);
        }


        while (!firingComplete)
        {
            yield return null;
        }

    }

    public void Hit()
    {
        firingComplete = true;
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
