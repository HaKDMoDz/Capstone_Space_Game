using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp_Wpn_Railgun : Component_Weapon
{
    //[SerializeField]
    //private Color railEffectStartColour = Color.white;
    //[SerializeField]
    //private float railEffectRadius = 1.5f;
    //[SerializeField]
    //private float railEffectDuration = 0.2f;
    //private LineRenderer line;
    //int length;

    [SerializeField]
    private Projectile_Missile projectilePrefab;
    [SerializeField]
    private LaserEffectController railEffectPrefab;
    [SerializeField]
    private float effectDuration = 1.0f;
    [SerializeField]
    private GameObject muzzleFlash;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float projectileSpeed = 10.0f;

    public override void Init(TurnBasedUnit _parentShip)
    {
        base.Init(_parentShip);
    }
    public override IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        //if(targetComp && targetComp.CompHP > 0.0f)
        {
            #if FULL_DEBUG
            //Debug.Log("Firing Railgun");
            #endif

            targetTrans = targetComp.transform;
            shootPoint.LookAt(targetTrans);
            //for laser style railgun
            //yield return StartCoroutine(CreateRailEffect());
            
            //for projectile railgun
            Projectile_Missile bulletClone = (Projectile_Missile)Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            bulletClone.rigidbody.velocity = shootPoint.forward * projectileSpeed;
            //float distanceToTarget = Vector3.Distance(targetTrans.position, shootPoint.position);
            //float timeToTarget = distanceToTarget / projectileSpeed;
            ////muzzle flash
            bool bulletCollided = false;
            bulletClone.OnCollision +=
                (GameObject other) =>
                {
                    if ((targetComp.ParentShip.ShieldStrength > 0.0f
                    && other.layer == TagsAndLayers.ShipShieldLayer
                    && other.GetComponentInParent<TurnBasedUnit>() == targetComp.ParentShip)
                    || (other.layer == TagsAndLayers.ComponentsLayer
                    && other.GetComponent<ShipComponent>() == targetComp))
                    {
                        bulletCollided = true;
                        Instantiate(explosionPrefab, bulletClone.transform.position, Quaternion.identity);
                        Destroy(bulletClone.gameObject);
                    }
                };
            //float currentTimer = 
            while (!bulletCollided)
            {
                yield return null;
            }
            Debug.Log("Weapon doing damage");
            yield return StartCoroutine(DoDamage(targetComp));
        }
        Debug.Log("Weapon activation complete");
        OnActivationComplete();
    }

    private IEnumerator CreateRailEffect()
    {
        //Vector3 targetDir = (targetTrans.position - shootPoint.position).normalized;
        LaserEffectController railClone = (LaserEffectController)Instantiate(railEffectPrefab, shootPoint.position, shootPoint.rotation);
        yield return StartCoroutine(railClone.PlayLaserEffect(effectDuration, targetTrans.position));
        //yield return new WaitForSeconds(effectDuration);

        Destroy(railClone.gameObject);

        //line.enabled = true;
        //int factor = 2;
        //line.SetVertexCount(length * factor);
        ////Vector3 targetDir = (targetTrans.position - shootPoint.position).normalized;

        //for (int i = 0; i < length * factor; i++)
        //{
        //    Vector3 newPos = shootPoint.position;
        //    float offsetX = railEffectRadius * Mathf.Cos(i + Time.time);
        //    float offsetY = railEffectRadius * Mathf.Sin(i + Time.time);

        //    newPos += shootPoint.right * offsetX;
        //    newPos += shootPoint.up * offsetY;
        //    newPos += shootPoint.forward * (float)i / factor;

        //    line.SetPosition(i, newPos);
        //}
        //Color currentColour = railEffectStartColour;
        //Color targetColour = currentColour.WithAplha(0.0f);
        //float currentTime = 0.0f;
        //while (currentTime <= railEffectDuration)
        //{
        //    line.SetColors(currentColour, currentColour);
        //    currentColour = Color.Lerp(currentColour, targetColour, currentTime);
        //    currentTime += Time.deltaTime;
        //    yield return null;
        //}
        //line.enabled = false;
    }
}