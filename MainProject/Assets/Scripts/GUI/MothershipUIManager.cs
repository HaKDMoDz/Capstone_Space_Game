using UnityEngine;
using System.Collections;

public class MothershipUIManager : MonoBehaviour 
{
    [SerializeField]
    private GameObject waypointUIElement;
    public GameObject WaypointUIElement
    {
        get { return waypointUIElement; }
        set { waypointUIElement = value; }
    }

    [SerializeField]
    private MissionSelector waypointUIScript;
    public MissionSelector WaypointUIScript
    {
        get { return waypointUIScript; }
        set { waypointUIScript = value; }
    }
    [SerializeField]
    private Transform systemDestination;
    public Transform SystemDestination
    {
        get { return systemDestination; }
        set { systemDestination = value; }
    }

    [SerializeField]
    private Transform planetDestination;
    public Transform PlanetDestination
    {
        get { return planetDestination; }
        set { planetDestination = value; }
    }
    public void enableWaypointUI( Transform _destination)
    {
        waypointUIElement.SetActive(true);
        planetDestination = _destination;
        waypointUIScript.CurrentDestination = planetDestination;
    }

    public void disableWaypointUI()
    {
        waypointUIElement.SetActive(false);
    }
}
