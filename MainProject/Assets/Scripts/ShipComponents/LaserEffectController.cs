/*
  LaserEffectController.cs
  Mission: Invasion
  Created by Rohun Banerji on March 09, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LaserEffectController : MonoBehaviour 
{
    [SerializeField]
    private LaserEffect laser1;
    [SerializeField]
    private LaserEffect laser2;
    [SerializeField]
    private GameObject beamParticleEffect;
    [SerializeField]
    private string forwardPlasmaName;
    [SerializeField]
    private float particleVelocity = 200.0f;

    public IEnumerator PlayLaserEffect(float duration, Vector3 impactPos)
    {
        StartCoroutine(laser1.PlayLaserEffect(duration, impactPos));
        StartCoroutine(laser2.PlayLaserEffect(duration, impactPos));
        GameObject particles = (GameObject)Instantiate(beamParticleEffect, transform.position, transform.rotation);
        particles.transform.LookAt(impactPos);
        ParticleSystem forwardPlasma = particles.GetComponentsInChildren<ParticleSystem>().FirstOrDefault(p => p.name == forwardPlasmaName);
        forwardPlasma.startLifetime = Vector3.Distance(impactPos, particles.transform.position) / particleVelocity;
        forwardPlasma.startLifetime *= 2.0f;
        Debug.Log("particle lifetime: " + forwardPlasma.startLifetime);
        Destroy(particles, duration);
        yield return new WaitForSeconds(duration);
    }

}
