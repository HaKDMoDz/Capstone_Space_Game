using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp_Wpn_Railgun : Component_Weapon
{
    [SerializeField]
    private Color railEffectStartColour = Color.white;
    [SerializeField]
    private float railEffectRadius = 1.5f;
    [SerializeField]
    private float railEffectDuration = 0.2f;

    private LineRenderer line;
    int length;

    public override void Init(TurnBasedUnit _parentShip)
    {
        base.Init(_parentShip);

        line = GetComponentInChildren<LineRenderer>();
        #if FULL_DEBUG
        if(!line)
        {
            Debug.LogError("No LineRenderer found");
        }
        #endif

        line.enabled = false;
    }
    public override IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        if(targetComp && targetComp.CompHP > 0.0f)
        {
            #if FULL_DEBUG
            //Debug.Log("Firing Railgun");
            #endif

            targetTrans = targetComp.transform;
            shootPoint.LookAt(targetTrans);
            length = Mathf.RoundToInt(Vector3.Distance(targetTrans.position, shootPoint.position));
            yield return StartCoroutine(CreateRailEffect());
            yield return StartCoroutine(DoDamage(targetComp));
        }
        OnActivationComplete();
    }

    private IEnumerator CreateRailEffect()
    {
        line.enabled = true;
        int factor = 2;
        line.SetVertexCount(length*factor);
        //Vector3 targetDir = (targetTrans.position - shootPoint.position).normalized;

        for (int i = 0; i < length*factor; i++)
        {
            Vector3 newPos = shootPoint.position;
            float offsetX = railEffectRadius * Mathf.Cos(i + Time.time);
            float offsetY = railEffectRadius * Mathf.Sin(i + Time.time);

            newPos += shootPoint.right * offsetX;
            newPos += shootPoint.up * offsetY;
            newPos += shootPoint.forward * (float)i / factor;

            line.SetPosition(i, newPos);
        }
        Color currentColour = railEffectStartColour;
        Color targetColour = currentColour.WithAplha(0.0f);
        float currentTime = 0.0f;
        while(currentTime <= railEffectDuration)
        {
            line.SetColors(currentColour, currentColour);
            currentColour = Color.Lerp(currentColour, targetColour, currentTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        line.enabled = false;
    }
}