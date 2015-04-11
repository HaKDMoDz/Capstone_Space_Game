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
    private GameObject uiRing;
    public GameObject UIRing
    {
        get { return uiRing; }
        set { uiRing = value; }
    }

    [SerializeField]
    private GameObject missionButton;
    public GameObject MissionButton
    {
        get { return missionButton; }
        set { missionButton = value; }
    }
    [SerializeField]
    private GameObject dialogueButton;
    public GameObject DialogueButton
    {
        get { return dialogueButton; }
        set { dialogueButton = value; }
    }

    [SerializeField]
    private Planet_Dialogue planetDialog;
    public Planet_Dialogue PlanetDialog
    {
        get { return planetDialog; }
        set { planetDialog = value; }
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

    public IEnumerator enableUIRing()
    {
        //uiRing.SetActive(true);

        gameObject.GetComponent<Planet_Mission>().Completed = GameController.Instance.GameData.galaxyMapData.completeStatus[gameObject.GetComponent<Planet_Mission>().ID - 1];
        //check for missions
        if (gameObject.GetComponent<Planet_Mission>() != null)
        {
            missionButton.SetActive(!gameObject.GetComponent<Planet_Mission>().Completed);
        }

        //check for dialog
        if (planetDialog.DialogueText.Length > 0)
        {
            dialogueButton.SetActive(true);
        }

        yield return null;
    }

    public IEnumerator disableUIRing()
    {
        dialogueButton.SetActive(false);
        missionButton.SetActive(false);
        uiRing.SetActive(false);

        yield return null;
    }

    public void disableMissionButton()
    {
        missionButton.SetActive(false);
    }

    public void enableMissionPanel()
    {
        missionPanel.SetActive(true);
        GetComponent<Planet_Mission>().advanceStartText();
    }

    public void disableMissionPanel()
    {
        missionPanel.SetActive(false);
    }
}
