/*
  MainMenuMotherShip.cs
  Mission: Invasion
  Created by Rohun Banerji on March 19, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuMotherShip : MonoBehaviour
{
    //Editor Exposed
    [SerializeField]
    private Transform orbitingPlanet;
    [SerializeField]
    private float orbitRadius = 50.0f;
    [SerializeField]
    private float orbitSpeed = 10.0f;
    [SerializeField]
    private Transform[] nukeLaunchers;
    [SerializeField]
    private Projectile_Missile nukePrefab;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private Vector2 nukeInterval = new Vector2(0.5f, 1.5f);


    //References
    private Transform trans;
    private IEnumerator OrbitPlanet()
    {
        StartCoroutine(StartNukingPlanet());
        while (true)
        {
            float xOffset = orbitRadius * Mathf.Sin(Time.time * orbitSpeed);
            float yOffset = orbitRadius * Mathf.Cos(Time.time * orbitSpeed);
            trans.position = orbitingPlanet.position + new Vector3(xOffset, 0.0f, yOffset);
            Vector3 dirToPlanet = orbitingPlanet.position - trans.position;
            Vector3 orbitTangent = Vector3.Cross(trans.up, dirToPlanet);
            trans.rotation = Quaternion.LookRotation(orbitTangent);
            yield return null;
        }
    }
    private IEnumerator StartNukingPlanet()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(nukeInterval.x, nukeInterval.y));
            NukePlanet();
        }
    }
    private void NukePlanet()
    {
        int launcherIndex = Random.Range(0, nukeLaunchers.Length);
        Transform launcher = nukeLaunchers[launcherIndex];
        Projectile_Missile nukeClone = (Projectile_Missile)Instantiate(nukePrefab, launcher.position, launcher.rotation);
        nukeClone.OnCollision +=
            (GameObject other) =>
            {
                if (other.transform == orbitingPlanet)
                {
                    Instantiate(explosionPrefab, nukeClone.transform.position, Quaternion.identity);
                    Destroy(nukeClone.gameObject);
                }
                #if FULL_DEBUG
                else
                {
                    Debug.LogError("Nuke hit something but the planet");
                }
                #endif
            };
    }

    private IEnumerator Start()
    {
        trans = transform;
        yield return StartCoroutine(OrbitPlanet());
    }

}
