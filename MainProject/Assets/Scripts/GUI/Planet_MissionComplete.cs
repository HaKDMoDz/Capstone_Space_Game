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



    public void toggleMissionCompletePanel()
    {
        panelOpen = !panelOpen;
        transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").gameObject.SetActive(panelOpen);
        transform.FindChild("PlanetUI").FindChild("MissionCompletePanel").FindChild("Text").GetComponent<Text>().text = completeText;
        
        if (!panelOpen)
        {
            transform.Find("PlanetUI").FindChild("MissionCompletePanel").gameObject.SetActive(false);
        }
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
