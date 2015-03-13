using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class Comp_Wpn_Missile : Component_Weapon
{
    [SerializeField]
    private Projectile_Missile missilePrefab;
    [SerializeField]
    private GameObject explosionPrefab;

    public override void Init(TurnBasedUnit _parentShip)
    {
        base.Init(_parentShip);
    }

    public override IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        //if (targetComp)
        {
            targetTrans = targetComp.transform;
            shootPoint.LookAt(targetTrans);
            Projectile_Missile missileClone = Instantiate(missilePrefab, shootPoint.position, shootPoint.rotation) as Projectile_Missile;
            bool missileCollided = false;
            missileClone.OnCollision +=
                (GameObject other) =>
                {
                    if ((targetComp.ParentShip.ShieldStrength > 0.0f
                        && other.layer == TagsAndLayers.ShipShieldLayer
                        && other.GetComponentInParent<TurnBasedUnit>() == targetComp.ParentShip)
                        || (other.layer == TagsAndLayers.ComponentsLayer
                        && other.GetComponent<ShipComponent>() == targetComp))
                    {
                        missileCollided = true;
                        Instantiate(explosionPrefab, missileClone.transform.position, Quaternion.identity);
                        Destroy(missileClone.gameObject);
                    }
                };
            while (!missileCollided && targetComp.ParentShip && targetComp.ParentShip.HullHP > 0.0f)
            {
                //Debug.Log("targetship hp " + targetComp.ParentShip.HullHP);
                yield return null;
            }
            yield return StartCoroutine(DoDamage(targetComp));

            OnActivationComplete();
        }
    }


}