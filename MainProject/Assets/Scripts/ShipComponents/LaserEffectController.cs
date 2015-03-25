/*
  LaserEffectController.cs
  Mission: Invasion
  Created by Rohun Banerji on March 09, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserEffectController : MonoBehaviour 
{
    [SerializeField]
    private LaserEffect laser1;
    [SerializeField]
    private LaserEffect laser2;
    [SerializeField]
    private GameObject beamParticleEffect;

    public IEnumerator PlayLaserEffect(float duration, Vector3 impactPos)
    {
        StartCoroutine(laser1.PlayLaserEffect(duration, impactPos));
        StartCoroutine(laser2.PlayLaserEffect(duration, impactPos));
        if (beamParticleEffect)
        {
            GameObject particles = (GameObject)Instantiate(beamParticleEffect, transform.position, transform.rotation);
            particles.transform.LookAt(impactPos);
            Destroy(particles, duration);
        }
        yield return new WaitForSeconds(duration);
    }

}
