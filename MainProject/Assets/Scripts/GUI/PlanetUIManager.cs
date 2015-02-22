using UnityEngine;
using System.Collections;

public class PlanetUIManager : MonoBehaviour 
{
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
    private GameObject missionCompleteButton;
    public GameObject MissionCompleteButton
    {
        get { return missionCompleteButton; }
        set { missionCompleteButton = value; }
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
    private GameObject missionPanel;
    public GameObject MissionPanel
    {
        get { return missionPanel; }
        set { missionPanel = value; }
    }

    [SerializeField]
    private GameObject missionCompletePanel;
    public GameObject MissionCompletePanel
    {
        get { return missionCompletePanel; }
        set { missionCompletePanel = value; }
    }

    public IEnumerator enableUIRing()
    {
        uiRing.SetActive(true);
        //check for missions
        if (gameObject.GetComponent<Planet_Mission>() != null)
        {
            missionButton.SetActive(true);
        }
        
        //check for mission complete
        

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
        missionCompleteButton.SetActive(false);
        uiRing.SetActive(false);

        yield return null;
    }

    public void enableMissionPanel()
    {
        missionPanel.SetActive(true);
    }

    public void disableMissionCompleteButton()
    {
        missionCompleteButton.SetActive(false);
    }

    public void disableMissionPanel()
    {
        missionPanel.SetActive(false);
    }

    public void enableMissionCompletePanel()
    {
        missionCompletePanel.SetActive(true);
    }

    public void disableMissionCompletePanel()
    {
        missionCompletePanel.SetActive(false);
    }
}
