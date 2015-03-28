/*
  MainMenuCamera.cs
  Mission: Invasion
  Created by Rohun Banerji on March 20, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuCamera : Singleton<MainMenuCamera> 
{
    [SerializeField]
    private Transform orbitingPlanet;
    [SerializeField]
    private float orbitSpeed = 5.0f;
    [SerializeField]
    private float orbitRadius = 120.0f;
    [SerializeField]
    private Vector2 swingAngles = new Vector2(90.0f, 270.0f);

    private bool atFirstPoint = true;
    //private Vector3 firstPoint, secondPoint;
    private Transform trans;

    public void SwingOver()
    {
        StartCoroutine(SwingToOtherSide());
    }

    private IEnumerator  SwingToOtherSide()
    {
        float currentAngle, destAngle;
        if(atFirstPoint)
        {
            currentAngle = swingAngles.x;
            destAngle = swingAngles.y;
        }
        else
        {
            currentAngle = swingAngles.y;
            destAngle = swingAngles.x + 360.0f;
        }
        currentAngle *= Mathf.Deg2Rad;
        destAngle *= Mathf.Deg2Rad;
        //not at dest angle
        while(Mathf.Abs(destAngle-currentAngle)> Mathf.Pow(GlobalVars.LerpDistanceEpsilon,4.0f))
        {
            float xOffset = orbitRadius * Mathf.Sin(currentAngle);
            float yOffset = orbitRadius * Mathf.Cos(currentAngle);
            trans.position = orbitingPlanet.position + new Vector3(xOffset, 0.0f, yOffset);
            trans.LookAt(orbitingPlanet);
            currentAngle = Mathf.Lerp(currentAngle, destAngle, orbitSpeed * Time.deltaTime);
            yield return null;
        }
        float x = orbitRadius * Mathf.Sin(destAngle);
        float y = orbitRadius * Mathf.Cos(destAngle);
        trans.position = orbitingPlanet.position + new Vector3(x, 0.0f, y);
        trans.LookAt(orbitingPlanet);
        atFirstPoint = !atFirstPoint;
    }
    private void Awake()
    {
        trans = transform;
        trans.LookAt(orbitingPlanet);
    }
}
