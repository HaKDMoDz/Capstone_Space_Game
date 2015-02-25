using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MissionController : Singleton<MissionController>
{
    public static int currentMissionIndex;

    public Planet_Mission currentMission;
    private List<Planet_Mission> allMissions;

    private Action[] acceptMissionFunctions = new Action[10];
    private Action[] completeMissionFunctions = new Action[10];

    public Transform CurrentDestination;

    public void Start()
    {
        acceptMissionFunctions[0] = null;
       

    }

    public void AddPlanetMission(Planet_Mission _planetMission)
    {
        if (allMissions == null)
        {
            allMissions = new List<Planet_Mission>();
        }

        allMissions.Add(_planetMission);
        _planetMission.startSystem.GetComponent<SolarSystem>().SystemRingGUI.GetComponent<SystemMissionIndicator>().Indicator.SetActive(true);
    }

    public void AddMission(int _index, Action F)
    {
        acceptMissionFunctions[_index] = F;
    }
    public void AddMissionComplete(int _index, Action F)
    {
        completeMissionFunctions[_index] = F;
    }

    public void AcceptMission(int _index)
    {
        Action acceptMission = acceptMissionFunctions[_index];
        acceptMission();
        currentMissionIndex = _index;
        Debug.Log("Accepted Mission: " + _index);
    }



    public void CompleteMission(int _index)
    {
        Action completeMission = completeMissionFunctions[_index];
        completeMission();
        Debug.Log("completing Mission: " + _index);
        currentMission.startSystem.GetComponent<SolarSystem>().SystemRingGUI.GetComponent<SystemMissionIndicator>().Indicator.SetActive(false);
    }
}
