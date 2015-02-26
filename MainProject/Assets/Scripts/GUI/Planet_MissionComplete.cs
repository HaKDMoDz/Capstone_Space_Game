﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Planet_MissionComplete : MonoBehaviour
{
    bool panelOpen = false;

    private Transform missionUIPanel;

    void Awake()
    {
        //ID = ++numMissions;
    }

    public void CompleteMission()
    {
        Debug.Log("Mission Completed");
        MissionController.Instance.CompleteMission(MissionController.currentMissionIndex);
        Debug.LogError(MissionController.Instance.currentMission.EndPlanet.GetComponent<PlanetUIManager>());
        MissionController.Instance.currentMission.EndPlanet.GetComponent<PlanetUIManager>().disableMissionCompleteButton();
        MissionController.Instance.currentMission.EndPlanet.GetComponent<PlanetUIManager>().disableMissionCompletePanel();
        //Debug.LogError(GameObject.Find("Mothership").GetComponent<MothershipUIManager>()); //linkage test
        //mothershipUI.disableWaypointUI(); //causes a null reference exception
        GameObject.Find("Mothership").GetComponent<MothershipUIManager>().disableWaypointUI(); //same code with new linkage
        MissionController.Instance.currentMission.Completed = true;
        MissionController.Instance.currentMission = null;
    }
}