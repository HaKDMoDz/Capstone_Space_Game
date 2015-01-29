using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Comp_Wpn_Laser : Component_Weapon 
{
    [SerializeField]
    private float effectDuration = 0.4f;
    [SerializeField]
    private ParticleSystem laserImpactEffect;
    [SerializeField]
    private float lineNoise;

    private LineRenderer line;
    int length;

    public override void Init()
    {
        base.Init();

        line = GetComponentInChildren<LineRenderer>();
        #if FULL_DEBUG
        if(!line)
        {
            Debug.LogError("No line renderer found");
        }
        #endif

        line.enabled = false;
        //laserImpactEffect.Stop();
    }

    public override IEnumerator Fire(Transform target, Action OnActivationComplete)
    {
        //length = Mathf.RoundToInt(Vector3.Distance(target.position, shootPoint.position));
        Debug.Log("Firing lasers");
        yield return null;
    }

    private void CreateBeamEffect()
    {
        //line.enabled = true;
        //line.SetVertexCount(length);
        //for (int i = 0; i < length; i++)
        //{
        //    Vector3 newPos = shootPoint.position;
        //    Vector3 offset = Vector3.zero;
        //    offset.x = newPos
        //}
    }
}
