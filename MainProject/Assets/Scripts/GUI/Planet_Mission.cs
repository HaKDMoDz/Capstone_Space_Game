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
    public GameObject StartSystem
    {
        get { return startSystem; }
        set { startSystem = value; }
    }
    [SerializeField]
    public GameObject endSystem;
    public GameObject EndSystem
    {
        get { return endSystem; }
        set { endSystem = value; }
    }
    [SerializeField]
    public GameObject startPlanet;
    public GameObject StartPlanet
    {
        get { return startPlanet; }
        set { startPlanet = value; }
    }
    [SerializeField]
    private GameObject endPlanet;
    public GameObject EndPlanet
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

    [SerializeField]
    public int rewardAmount;
    public int RewardAmount
    {
        get { return rewardAmount; }
        set { rewardAmount = value; }
    }
    [SerializeField]
    private bool completed = false;
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
        Completed = false;
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

        MissionController.Instance.AddPlanetMission(this);
	}

    public void CompleteMission()
    {
        Debug.Log("Mission Completed");
        MissionController.Instance.CompleteMission(MissionController.currentMissionIndex);
        uiManager.disableMissionCompleteButton();
        uiManager.disableMissionCompletePanel();
        //Debug.LogError(GameObject.Find("Mothership").GetComponent<MothershipUIManager>()); //linkage test
        //mothershipUI.disableWaypointUI(); //causes a null reference exception
        GameObject.Find("Mothership").GetComponent<MothershipUIManager>().disableWaypointUI(); //same code with new linkage
        MissionController.Instance.currentMission.Completed = true;
        MissionController.Instance.currentMission = null;
    }

    public void advanceStartText()
    {
        transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text").GetComponent<Text>().text = startDialog[startDialogIndex++];
    }

    public void advanceEndText()
    {
        endPlanet.transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").FindChild("Text").GetComponent<Text>().text = endDialog[endPlanet.GetComponent<Planet_MissionComplete>().endDialogIndex++];
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
            mothershipUI.enableWaypointUI(endPlanet.transform);
            MissionController.Instance.currentMission = this;
            uiManager.disableMissionButton();
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
    }

    private void missioncomplete2()
    {
        PlanetUIManager planetUIManager = endPlanet.GetComponent<PlanetUIManager>();
        planetUIManager.disableMissionCompleteButton();
        planetUIManager.disableMissionCompletePanel();
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
