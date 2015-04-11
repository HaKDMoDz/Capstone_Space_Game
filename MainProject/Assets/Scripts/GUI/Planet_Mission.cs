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
    public GameObject mothership;

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

    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

	void Awake () 
    {
        Action acceptAction;
        switch (ID)
        {
            case 1:
                acceptAction = (() => invasionPlanet01());
                break;
            case 2:
                acceptAction = (() => invasionPlanet02());
                break;
            case 3:
                acceptAction = (() => invasionPlanet03());
                break;
            case 4:
                acceptAction = (() => invasionPlanet04());
                break;
            case 5:
                acceptAction = (() => invasionPlanet05());
                break;
            case 6:
                acceptAction = (() => invasionPlanet06());
                break;
            case 7:
                acceptAction = (() => invasionPlanet07());
                break;
            case 8:
                acceptAction = (() => invasionPlanet08());
                break;
            case 9:
                acceptAction = (() => invasionPlanet09());
                break;
            case 10:
                acceptAction = (() => invasionPlanet10());
                break;
            case 0: default:
                acceptAction = (() => invalidMission());
                break;
        }
        MissionController.Instance.AddMission(ID, acceptAction);
        MissionController.Instance.AddPlanetMission(this);
	}

    public void advanceStartText()
    {
        if (startDialog.Count != 0)
        {
            transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text").GetComponent<Text>().text = startDialog[startDialogIndex++];
            uiManager.MissionText.text = startDialog[startDialogIndex++];
        }
    }

    public void AcceptMission()
    {
        if(startDialogIndex < startDialog.Count)
        {
            advanceStartText();
        }
        else
        {
            GameController.Instance.GameData.galaxyMapData.currentMissionID = ID;
            GameController.Instance.GameData.galaxyMapData.position = mothershipUI.transform.position;
            MissionController.Instance.currentMission = this;
            uiManager.disableMissionButton();
            MissionController.Instance.AcceptMission(ID);
        }
    }

    private void invalidMission()
    {
        Debug.LogError("AcceptMission: Invalid Mission ID: " + ID);
    }
    private void invalidMissionComplete()
    {
        Debug.LogError("CompleteMission: Invalid Mission ID: " + ID);
    }

    private void invasionPlanet01()
    {
        Debug.Log("Click");
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet02()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("NewAIFrig");
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("NewAICorv");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet03()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet04()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet05()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet06()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet07()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet08()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet09()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }

    private void invasionPlanet10()
    {
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames = new List<string>();
        GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Add("K-104B");
        GameController.Instance.ChangeScene(GameScene.CombatScene);
    }
}
