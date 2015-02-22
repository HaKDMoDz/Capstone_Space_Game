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
    public IEnumerator enableWaypointUI()
    {
        waypointUIElement.SetActive(true);

        yield return null;
    }

    public IEnumerator disableWaypointUI()
    {
        waypointUIElement.SetActive(false);

        yield return null;
    }
}
