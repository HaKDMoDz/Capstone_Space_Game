using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Planet_Mission : MonoBehaviour 
{
    [SerializeField]
    public MothershipUIManager mothershipUI;

    [SerializeField]
    public PlanetUIManager uiManager;

    [SerializeField]
    public GameObject startSystem;
    public UnityEngine.GameObject StartSystem
    {
        get { return startSystem; }
        set { startSystem = value; }
    }
    [SerializeField]
    public GameObject endSystem;
    public UnityEngine.GameObject EndSystem
    {
        get { return endSystem; }
        set { endSystem = value; }
    }
    [SerializeField]
    public GameObject startPlanet;
    public UnityEngine.GameObject StartPlanet
    {
        get { return startPlanet; }
        set { startPlanet = value; }
    }
    [SerializeField]
    public GameObject endPlanet;
    public UnityEngine.GameObject EndPlanet
    {
        get { return endPlanet; }
        set { endPlanet = value; }
    }
    [SerializeField]
    public List<string> startDialog;
    public List<string> StartDialog
    {
        get { return startDialog; }
        set { startDialog = value; }
    }
    private int startDialogIndex;
    [SerializeField]
    public List<string> endDialog;
    public List<string> EndDialog
    {
        get { return endDialog; }
        set { endDialog = value; }
    }
    private int endDialogIndex;
    [SerializeField]
    public int rewardAmount;
    public int RewardAmount
    {
        get { return rewardAmount; }
        set { rewardAmount = value; }
    }
    [SerializeField]
    private bool completed;
    public bool Completed
    {
        get { return completed; }
        set { completed = value; }
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

        Action acceptAction;
        switch (ID)
        {
            case 5:
                acceptAction = (() => mission1());
                break;
            case 1:
                acceptAction = (() => mission2());
                break;
            case 0: default:
                acceptAction = (() => invalidMission());
                break;
        }
        MissionController.Instance.AddMission(ID, acceptAction);

        Action completeAction;

        switch (ID)
        {
            case 5:
                completeAction = (() => mission1());
                break;
            case 1:
                completeAction = (() => missioncomplete2());
                break;
            case 0:
            default:
                completeAction = (() => invalidMissionComplete());
                break;
        }
        MissionController.Instance.AddMissionComplete(ID, completeAction);
	}

    public void CompleteMission()
    {
        Debug.Log("Mission Completed");
        MissionController.Instance.CompleteMission(MissionController.currentMission);
        uiManager.disableMissionCompleteButton();
        uiManager.disableMissionCompletePanel();
        StartCoroutine(mothershipUI.disableWaypointUI());
    }

    private void advanceStartText()
    {
        transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text").GetComponent<Text>().text = startDialog[startDialogIndex++];
    }

    private void advanceEndText()
    {
        transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").FindChild("Text").GetComponent<Text>().text = endDialog[startDialogIndex++];
    }

    public void AcceptMission()
    {
        if(startDialogIndex < startDialog.Count)
        {
            advanceStartText();
        }
        else
        {
            Debug.Log("Mission Accepted");
            MissionController.Instance.AcceptMission(ID);
            uiManager.disableMissionPanel();
            StartCoroutine(mothershipUI.enableWaypointUI());
        }
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
        mothershipUI.PlanetDestination = endPlanet.transform;
        mothershipUI.SystemDestination = endSystem.transform;
        PlanetUIManager endPlanetUI = endPlanet.GetComponent<PlanetUIManager>();
        endPlanetUI.MissionCompleteButton.SetActive(true);
        completed = true;
    }

    private void missioncomplete2()
    {

        PlanetUIManager planetUIManager = endPlanet.GetComponent<PlanetUIManager>();
        planetUIManager.disableMissionCompleteButton();
        planetUIManager.disableMissionCompletePanel();

        //uiManager.disableMissionCompletePanel();
    }

    private void invalidMission()
    {
        Debug.LogError("AcceptMission: Invalid Mission ID: " + ID);
    }
    private void invalidMissionComplete()
    {
        Debug.LogError("CompleteMission: Invalid Mission ID: " + ID);
    }
	
}
