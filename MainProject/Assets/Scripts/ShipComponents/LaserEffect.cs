/*
  LaserEffect.cs
  Mission: Invasion
  Created by Rohun Banerji on March 09, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserEffect : MonoBehaviour 
{
    [SerializeField]
    private LineRenderer laser;
    [SerializeField]
    private float startWidth;
    [SerializeField]
    private float endWidth;
    [SerializeField]
    private Color startColour = Color.red.WithAplha(0.3f);
    [SerializeField]
    private Color endColour = Color.red.WithAplha(0.0f);
    [SerializeField]
    private float uvLength = 4.0f;

    private Material mat;

    public IEnumerator PlayLaserEffect(float effectDuration, Vector3 impactPos)
    {
        #if FULL_DEBUG
        if(!laser)
        {
            Debug.LogError("Line renderer null");
        }
        #endif
        laser.SetPosition(0, transform.position);
        laser.SetPosition(1, impactPos);
        float distance = Vector3.Distance(transform.position, impactPos);
        renderer.materials[0].mainTextureScale.Set(distance / uvLength, renderer.materials[0].mainTextureScale.y) ;
        mat = laser.renderer.material;

        float currentTime = 0.0f;
        while(currentTime<=1.0f)
        {
            //lerp colour
            mat.SetColor("_TintColor", Color.Lerp(startColour, endColour, currentTime));
            //lerp size
            float newSize = Mathf.Lerp(startWidth, endWidth, currentTime);
            laser.SetWidth(newSize, newSize); 
            currentTime += Time.deltaTime / effectDuration;
            yield return null;
        }

    }

}
