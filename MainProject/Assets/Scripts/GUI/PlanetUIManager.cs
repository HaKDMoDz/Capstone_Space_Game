using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlanetUIManager : MonoBehaviour 
{
    [SerializeField]
    private Text missionText;
    public Text MissionText
    {
        get { return missionText; }
        set { missionText = value; }
    }

    [SerializeField]
    private GameObject missionButton;
    public GameObject MissionButton
    {
        get { return missionButton; }
        set { missionButton = value; }
    }

    [SerializeField]
    private Planet_Mission planetMission;
    public Planet_Mission PlanetMission
    {
        get { return planetMission; }
        set { planetMission = value; }
    }

    [SerializeField]
    private GameObject missionPanel;
    public GameObject MissionPanel
    {
        get { return missionPanel; }
        set { missionPanel = value; }
    }

    public IEnumerator enableUI()
    {
        //check for missions
        if (planetMission != null)
        {
            planetMission.Completed = GameController.Instance.GameData.galaxyMapData.completeStatus[planetMission.ID - 1];
            missionButton.SetActive(!planetMission.Completed);
        }

        yield return null;
    }

    public IEnumerator disableUI()
    {
        missionButton.SetActive(false);

        yield return null;
    }

    public void disableMissionButton()
    {
        missionButton.SetActive(false);
    }

    public void enableMissionPanel()
    {
        missionPanel.SetActive(true);
        planetMission.advanceStartText();
    }

    public void disableMissionPanel()
    {
        missionPanel.SetActive(false);
    }
}
