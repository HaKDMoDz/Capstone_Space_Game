/*
  ShipMove.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 17/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipShield : MonoBehaviour 
{
    [SerializeField]
    private float effectDuration = 1.0f;
    [SerializeField]
    private float effectSpeed = 0.1f;
    public TurnBasedUnit ParentShip { get; private set; }
    
    private Material shieldMat;
    private Transform trans;
    private Color originalColour; 

    public void TakeDamage(Vector3 hitPoint)
    {
        Vector3 localHitPoint = trans.InverseTransformPoint(hitPoint);
        //Debug.Log("Hit Point: " + hitPoint + " Local: " + localHitPoint);
        shieldMat.SetVector("_Position", localHitPoint.ToVector4());
        StartCoroutine(ShieldEffect());
    }

    private IEnumerator ShieldEffect()
    {
        float currentTime = 0.0f;

        shieldMat.SetColor("_Color", originalColour);
        Color currentColour = originalColour;
        Color targetColour = originalColour.WithAplha(0.0f);
        while(currentTime <= effectDuration)
        {
            shieldMat.SetFloat("_Offset", Mathf.Repeat(currentTime*effectSpeed, 1.0f));
            currentColour = Color.Lerp(currentColour, targetColour, currentTime);
            shieldMat.SetColor("_Color", currentColour);

            currentTime += Time.deltaTime;
            yield return null;
        }
        //gameObject.SetActive(false);
    }
    public void Init(TurnBasedUnit parentShip)
    {
        trans = transform;
        shieldMat = renderer.material;
        originalColour = shieldMat.GetColor("_Color");
        ParentShip = parentShip;
    }
    private void Awake()
    {
        
    }

}
