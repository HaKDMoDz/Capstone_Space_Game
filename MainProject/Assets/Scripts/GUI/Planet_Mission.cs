﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Planet_Mission : MonoBehaviour 
{

    public SystemObject destination;
    public GameObject[] targets;
    [SerializeField]
    private bool completed;
    public bool Completed
    {
        get { return completed; }
        set { completed = value; }
    }
    [SerializeField]
    private int creditReward;
    public int CreditReward
    {
        get { return creditReward; }
        set { creditReward = value; }
    }
    [SerializeField]
    private string missionText;
    public string MissionText
    {
        get { return missionText; }
        set { missionText = value; }
    }
    bool panelOpen = false;

    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    private Transform missionUIPanel;

	void Awake () 
    {
        //ID = ++numMissions;

        Action acceptAction;

        switch (ID)
        {
            case 1:
                acceptAction = (() => mission1());
                break;
            case 2:
                acceptAction = (() => mission2());
                break;
            case 0: default:
                acceptAction = (() => invalidMission());
                break;
        }
        MissionController.Instance.AddMission(ID, acceptAction);
	}

    public void showMissionPanel()
    {
        panelOpen = !panelOpen;
        transform.FindChild("PlanetUI").FindChild("MissionPanel").gameObject.SetActive(panelOpen);
        transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text").GetComponent<Text>().text = missionText;

    }

    public void AcceptMission()
    {
        Debug.Log("Mission Accepted");
        MissionController.Instance.AcceptMission(ID);
    }

    private void mission1()
    {
        Debug.Log("Mission 1 clicked");

        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Corvette");
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Corvette");
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Corvette");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void mission2()
    {
        Debug.Log("Mission 2 clicked");
        //GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        //GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Frigate");
        //GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Frigate");
        //GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("AI_Frigate");
        //GameController.Instance.ChangeScene(GameScene.CombatScene);
        destination.transform.FindChild("PlanetUI").gameObject.SetActive(true);
        destination.transform.FindChild("PlanetUI").FindChild("MissionCompleteButton").gameObject.SetActive(true);
        destination.GetComponent<Planet_MissionComplete>().InitMissionComplete(ID);
        GameObject.Find("missionSelector").SetActive(true);
        GameObject.Find("missionSelector").GetComponent<MissionSelector>().CurrentDestination = destination.transform;
        transform.FindChild("PlanetUI").FindChild("MissionButton").gameObject.SetActive(false);
        transform.FindChild("PlanetUI").FindChild("MissionPanel").gameObject.SetActive(false);
    }

    private void invalidMission()
    {
        Debug.LogError("AcceptMission: Invalid Mission ID: " + ID);
    }
	
}
