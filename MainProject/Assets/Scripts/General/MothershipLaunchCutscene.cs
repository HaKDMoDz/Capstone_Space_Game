/*
  MothershipLaunchCutscene.cs
  Mission: Invasion
  Created by Rohun Banerji on March 16, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MothershipLaunchCutscene : MonoBehaviour 
{
    [SerializeField]
    private GameObject[] canvases;
    [SerializeField]
    private Transform[] hangars;
    [SerializeField]
    private Transform launchPos;
    [SerializeField]
    private float timeToExitHangar = 2.5f;
    [SerializeField]
    private float shipMoveSpeed = 50.0f;
    [SerializeField]
    private float shipTurnSpeed = 1.0f;

    private Vector3 camMotherShipPos;
    private Quaternion camMotherShipRot;

    private IEnumerator PreCutscene()
    {
        //deactivate UI
        camMotherShipPos = Camera.main.transform.position;
        camMotherShipRot = Camera.main.transform.rotation;
        SpaceGround.Instance.Display(false);
        foreach (GameObject go in canvases)
        {
            go.SetActive(false);
        }
        
        yield return null;
    }
    public IEnumerator PlayCutscene(Dictionary<Transform, Vector3> ship_gridPos_Table)
    {
        yield return StartCoroutine(PreCutscene());
        foreach (var ship_gridPos in ship_gridPos_Table)
        {
            Transform shipTrans = ship_gridPos.Key;
            Vector3 destination = ship_gridPos.Value;
            shipTrans.position = hangars[0].position;
            shipTrans.rotation = hangars[0].rotation;
            yield return StartCoroutine(ExitHangar(shipTrans));
            //yield return StartCoroutine
        }

        while(!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        yield return StartCoroutine(PostCutscene());
    }
    private IEnumerator PostCutscene()
    {
        //re-activate UI
        SpaceGround.Instance.Display(true);
        foreach (GameObject go in canvases)
        {
            go.SetActive(true);
        }
        yield return null;
    }
    private IEnumerator FlyToGridPos(Transform ship, Vector3 destination)
    {
        Vector3 dirToDest = destination - ship.position;
        float timeToReachDest = dirToDest.magnitude / shipMoveSpeed;
        float halfPeriod = timeToReachDest * 0.5f;

        //rot when < half, destRot = lookrot(
        //while pos!=dest
            
            //rotation lerp towar
        yield return null;
    }
    private IEnumerator ExitHangar(Transform ship)
    {
        float time = 0.0f;
        Vector3 startPos = ship.position;
        Quaternion startRot = ship.rotation;
        while(time<1.0f)
        {
            ship.position = Vector3.Lerp(startPos, launchPos.position, time);
            ship.rotation = Quaternion.Slerp(startRot, launchPos.rotation, time);
            time += Time.deltaTime / timeToExitHangar;
            yield return null;
        }
    }

}
