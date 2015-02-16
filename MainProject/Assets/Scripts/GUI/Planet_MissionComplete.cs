using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Planet_MissionComplete : MonoBehaviour
{
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
    private string completeText;
    public string CompleteText
    {
        get { return completeText; }
        set { completeText = value; }
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

    void Awake()
    {
        //ID = ++numMissions;

        
    }

    public void InitMissionComplete(int _ID)
    {
        ID = _ID;
        Action completeAction;

        switch (ID)
        {
            case 1:
                completeAction = (() => mission1());
                break;
            case 2:
                completeAction = (() => mission2());
                break;
            case 0:
            default:
                completeAction = (() => invalidMission());
                break;
        }
        MissionController.Instance.AddMission(ID, completeAction);
    }

    public void toggleMissionCompletePanel()
    {
        panelOpen = !panelOpen;
        transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").gameObject.SetActive(panelOpen);
        transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").FindChild("Text").GetComponent<Text>().text = completeText;
        
        if (!panelOpen)
        {
            transform.Find("PlanetUI").FindChild("MissionCompleteButton").gameObject.SetActive(false);
        }

    }

    public void CompleteMission()
    {
        Debug.Log("Mission Completed");
        MissionController.Instance.CompleteMission(ID);
    }

    private void mission1()
    {
        Debug.Log("Mission 1 completed");
        toggleMissionCompletePanel();
        
    }

    private void mission2()
    {
        Debug.Log("Mission 2 completed");
        //GameObject.Find("missionSelector").SetActive(false);
        toggleMissionCompletePanel();
    }

    private void invalidMission()
    {
        Debug.LogError("CompleteMissionMission: Invalid Mission ID: " + ID);
    }

}
