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
    private GameObject canvas;
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
    [SerializeField]
    private float camTurnSpeed = 5.0f;
    [SerializeField]
    private GameObject skipText;
    private Vector3 camMotherShipPos;
    private Quaternion camMotherShipRot;
    [SerializeField]
    private Vector3 camPosOnTop;
    //private Quaternion camRotOnTop=Quaternion.Euler(30.88592f,0.0f,0.0f);
    private Transform camTrans;
    Dictionary<Transform, Vector3> ship_gridPos_Table;
    private bool skipCutscene = false;

    public void SkipCutScene()
    {
        SkipCutscene(KeyCode.Escape);
    }
    private void SkipCutscene(KeyCode key)
    {
        skipCutscene = true;
        PostCutscene();
    }
    private IEnumerator PreCutscene()
    {
        InputManager.Instance.RegisterKeysDown(SkipCutscene, KeyCode.Escape);
        //deactivate UI
        //camMotherShipPos = Camera.main.transform.position;
        camMotherShipPos = new Vector3(0.0f, 167.5f, -318.6f);
        //camMotherShipRot = Camera.main.transform.rotation;
        camMotherShipRot = Quaternion.Euler(30.88592f, 180.0f, 0.0f);
        SpaceGround.Instance.Display(false);
        canvas.SetActive(false);
        camTrans = Camera.main.transform;
        yield return null;
    }
    public IEnumerator PlayCutscene(Dictionary<Transform, Vector3> _ship_gridPos_Table)
    {
        this.ship_gridPos_Table = _ship_gridPos_Table;
        yield return StartCoroutine(PreCutscene());
        foreach (var ship_gridPos in ship_gridPos_Table)
        {
            Transform shipTrans = ship_gridPos.Key;
            int hangarIndex = Random.Range(0, hangars.Length);
            shipTrans.position = hangars[hangarIndex].position;
            shipTrans.rotation = hangars[hangarIndex].rotation;
        }
        for (int i = 0; i < ship_gridPos_Table.Count; i++)
        {
            if (skipCutscene) yield break;
            Transform shipTrans = ship_gridPos_Table.ElementAt(i).Key;
            Vector3 destination = ship_gridPos_Table.ElementAt(i).Value;
            yield return StartCoroutine(ExitHangar(shipTrans));
            if (skipCutscene) yield break;
            yield return StartCoroutine(FlyToGridPos(shipTrans, destination));
            if (skipCutscene) yield break;
            //swing back to mothership until last ship
            if (!skipCutscene && i < ship_gridPos_Table.Count - 1)
            {
                yield return StartCoroutine(CameraDirector.Instance.MoveAndRotate(camMotherShipPos, camMotherShipRot, 1.0f));
            }
            shipTrans.position = destination;
            shipTrans.rotation = Quaternion.identity;
        }
        PostCutscene();
    }
    private void PostCutscene()
    {
        //re-activate UI
        SpaceGround.Instance.Display(true);
        canvas.SetActive(true);
        skipText.SetActive(false);
        InputManager.Instance.DeregisterKeysDown(SkipCutscene, KeyCode.Escape);
    }
    private IEnumerator FlyToGridPos(Transform ship, Vector3 destination)
    {
        Vector3 initialPos = ship.position;
        Vector3 dirToDest = destination - ship.position;
        float dirMag = dirToDest.magnitude;
        float timeToReachDest = dirMag / shipMoveSpeed;
        //float halfPeriod = timeToReachDest * 0.5f;
        //assuming angle is the same as destination angle
        float dot = Vector3.Dot(ship.forward, dirToDest);
        bool isToTheRight = Vector3.Dot(ship.right, dirToDest) > 0.0f;
        float angle = Mathf.Acos(dot / dirMag) * Mathf.Rad2Deg;
        Debug.Log("Dot: " + dot / dirMag + " Angle: " + angle + " isRight " + isToTheRight);
        float doubleAngle = isToTheRight ? angle * 2.0f : angle * -2.0f;
        Quaternion halfDistRot = Quaternion.AngleAxis(doubleAngle, Vector3.up);
        Vector3 doubleAngleVec = halfDistRot * ship.forward;
        float time = 0.0f;
        while (time < 1.0f)
        {
            if (skipCutscene) yield break;
#if FULL_DEBUG
            Debug.DrawRay(initialPos, dirToDest, Color.red);
            Debug.DrawRay(ship.position, ship.forward * 500.0f, Color.green);
            Debug.DrawRay(initialPos, doubleAngleVec * 500.0f, Color.blue);
#endif
            if (time <= 0.5f)
            {
                ship.rotation = Quaternion.Slerp(ship.rotation, halfDistRot, time * 2.0f * shipTurnSpeed * Time.deltaTime);
            }
            else
            {
                ship.rotation = Quaternion.Slerp(ship.rotation, Quaternion.identity, (time * 2.0f - 1.0f) * shipTurnSpeed * Time.deltaTime);
            }
            ship.position += ship.forward * shipMoveSpeed * Time.deltaTime;
            time += Time.deltaTime / timeToReachDest;
            //Camera.main.transform.LookAt(ship);
            camTrans.rotation = Quaternion.Slerp(camTrans.rotation, Quaternion.LookRotation(ship.position - camTrans.position), camTurnSpeed * Time.deltaTime);
            yield return null;
        }
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
            if (skipCutscene) yield break;
            yield return null;
        }
    }

}
