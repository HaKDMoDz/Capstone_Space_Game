using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	void Awake () 
    {
        
	}

    public void showMissionPanel()
    {
        panelOpen = !panelOpen;
        transform.FindChild("PlanetUI").FindChild("MissionPanel").gameObject.SetActive(panelOpen);
        Debug.Log(transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text"));
        transform.FindChild("PlanetUI").FindChild("MissionPanel").FindChild("Text").GetComponent<Text>().text = missionText;

    }

    public void AcceptMission()
    {
        Debug.Log("Mission Accepted");
    }
	
}
