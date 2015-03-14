using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Comp_Wpn_Laser : Component_Weapon
{
    [SerializeField]
    private float effectDuration = 0.4f;
    [SerializeField]
    private ParticleSystem laserImpactEffect;
    [SerializeField]
    private float lineNoise;
    [SerializeField]
    private LaserEffectController laserEffectPrefab;


    //private LineRenderer line;
    //int length;
    

    public override void Init(TurnBasedUnit parentShip)
    {
        base.Init(parentShip);

        //line = GetComponentInChildren<LineRenderer>();
        //#if FULL_DEBUG
        //if (!line)
        //{
        //    Debug.LogError("No line renderer found");
        //}
        //#endif

        //line.enabled = false;
        //laserImpactEffect.Stop();
    }

    public override IEnumerator Fire(ShipComponent targetComp, System.Action OnActivationComplete)
    {
        if (targetComp && targetComp.CompHP > 0.0f)
        {
            #if FULL_DEBUG
            Debug.Log("Firing lasers");
            #endif

            targetTrans = targetComp.transform;
            //length = Mathf.RoundToInt(Vector3.Distance(targetTrans.position, shootPoint.position));

            yield return StartCoroutine(CreateBeamEffectForDuration());
            yield return StartCoroutine(DoDamage(targetComp));
        }
        OnActivationComplete();
    }

    /// <summary>
    /// Creates the beam effect for the predefined duration
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateBeamEffectForDuration()
    {
        //float currentTime = 0.0f;
        //line.enabled = true;
        //line.SetVertexCount(length);
        //Vector3 targetDir = (targetTrans.position - shootPoint.position).normalized;
        //while (currentTime <= effectDuration)
        //{
        //    CreateBeamEffect(targetDir);
        //    currentTime += Time.deltaTime;
        //    yield return null;
        //}
        //line.enabled = false;

        //Vector3 targetDir = (targetTrans.position - shootPoint.position).normalized;
        LaserEffectController laserClone = (LaserEffectController)Instantiate(laserEffectPrefab, shootPoint.position, shootPoint.rotation);
        yield return StartCoroutine(laserClone.PlayLaserEffect(effectDuration, targetTrans.position));
        Destroy(laserClone.gameObject);
    }

    //private void CreateBeamEffect(Vector3 targetDir)
    //{
    //    for (int i = 0; i < length; i++)
    //    {
    //        Vector3 newPos = shootPoint.position;
    //        Vector3 offset = Vector3.zero;
    //        offset.x = newPos.x + i * targetDir.x + Random.Range(-lineNoise, lineNoise);
    //        offset.y = newPos.y + i * targetDir.y + Random.Range(-lineNoise, lineNoise);
    //        offset.z = newPos.z + i * targetDir.z + Random.Range(-lineNoise, lineNoise);
    //        newPos = offset;
    //        line.SetPosition(i, newPos);
    //    }
    //}
}
