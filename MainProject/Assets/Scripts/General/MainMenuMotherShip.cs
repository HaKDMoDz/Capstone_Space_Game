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
    private float startAngle = 90.0f;
    [SerializeField]
    private float orbitRadius = 50.0f;
    [SerializeField]
    private float orbitSpeed = 10.0f;
    [SerializeField]
    private Transform[] nukeLaunchers;
    [SerializeField]
    private Projectile_Missile nukePrefab;
    [SerializeField]
    private GameObject[] explosionPrefabs;
    [SerializeField]
    private Vector2 nukeInterval = new Vector2(0.5f, 1.5f);
    
    private bool onFarSide = false;
    //References
    private Transform trans;
    private IEnumerator OrbitPlanet()
    {
        StartCoroutine(StartNukingPlanet());
        while (true)
        {
            float xOffset = orbitRadius * Mathf.Sin(startAngle + Time.time * orbitSpeed);
            float yOffset = orbitRadius * Mathf.Cos(startAngle + Time.time * orbitSpeed);
            trans.position = orbitingPlanet.position + new Vector3(xOffset, 0.0f, yOffset);
            Vector3 dirToPlanet = orbitingPlanet.position - trans.position;
            Vector3 orbitTangent = Vector3.Cross(trans.up, dirToPlanet);
            trans.rotation = Quaternion.LookRotation(orbitTangent);
            bool previousSide = onFarSide;
            onFarSide = Vector3.Dot(trans.right, orbitingPlanet.forward) >= 0.0f;
            if(onFarSide != previousSide)
            {
                MainMenuCamera.Instance.SwingOver();
            }
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
                    int explosionIndex = Random.Range(0, explosionPrefabs.Length);
                    Instantiate(explosionPrefabs[explosionIndex], nukeClone.transform.position, nukePrefab.transform.rotation);
                    bool effect1 = Random.value > 0.5f;
                    if(effect1)
                    {
                        AudioManager.Instance.PlayEffect(Sound.Nuke1, true);
                    }
                    else
                    {
                        AudioManager.Instance.PlayEffect(Sound.Nuke2, true);
                    }
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
        startAngle *= Mathf.Deg2Rad;
        yield return StartCoroutine(OrbitPlanet());
    }

}
