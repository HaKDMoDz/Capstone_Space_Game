using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class Comp_Wpn_Missile : Component_Weapon
{
    [SerializeField]
    private Projectile_Missile missilePrefab;

    public override void Init(TurnBasedUnit _parentShip)
    {
        base.Init(_parentShip);
    }

    public override IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        #if FULL_DEBUG
        //Debug.Log("Firing Missile");
        #endif
        targetTrans = targetComp.transform;
        shootPoint.LookAt(targetTrans);
        Projectile_Missile missileClone = Instantiate(missilePrefab, shootPoint.position, shootPoint.rotation) as Projectile_Missile;
        bool missileCollided = false;
        missileClone.OnCollision +=
            (GameObject other) =>
            {
                if (other.layer == TagsAndLayers.ComponentsLayer)
                {
                    if (other.GetComponent<ShipComponent>()==targetComp)
                    {
                        missileCollided = true;
                        Destroy(missileClone.gameObject);
                    }
                }
            };
        while (!missileCollided)
        {
            yield return null;
        }
        yield return StartCoroutine(DoDamage(targetComp));

        OnActivationComplete();

    }


}