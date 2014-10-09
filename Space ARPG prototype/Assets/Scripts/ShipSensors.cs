using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum shipSensorStates { OUTER_SPACE, STELLAR_ORBIT, PLANETARY_ORBIT}

public class ShipSensors : MonoBehaviour 
{
    private List<GameObject> objectsNearMe;
    private GameObject closestObjectToMe = null;
    private float timeBetweenSensorSweeps = 0.25f;
    private Transform playerShip;

	// Use this for initialization
	void Start () 
    {
        GetComponent<SphereCollider>().radius = 20.0f;
        objectsNearMe = new List<GameObject>();
        playerShip = GameObject.Find("PlayerShip").transform;
        StartCoroutine(SensorSweep());
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CelestialObject")
        {
            objectsNearMe.Add(other.gameObject);
        }
    }

    void OnTriggerExit( Collider other)
    {
        if (other.tag == "CelestialObject")
        {
            objectsNearMe.Remove(other.gameObject);
        }
    }

    IEnumerator SensorSweep()
    {
        CelestialObject currCOScript;

        foreach (GameObject gObject in objectsNearMe)
        {
            gObject.GetComponent<CelestialObject>().myDistanceToShip = (gObject.transform.position - playerShip.position).magnitude;

            if (closestObjectToMe != null)
            {
                if (gObject.GetComponent<CelestialObject>().myDistanceToShip < closestObjectToMe.GetComponent<CelestialObject>().myDistanceToShip)
                {
                    closestObjectToMe = gObject;
                }
            }
            else
            {
                closestObjectToMe = gObject;
            }

        }

        if (objectsNearMe.Count != 0)
        {
            currCOScript = closestObjectToMe.GetComponent<CelestialObject>();

            if (currCOScript.name == "Star")
            {
               // yield return StartCoroutine(CameraManager.Instance.changeZoomLevel(CameraManager.MED_FAR_ZOOM));
            }
            else if (currCOScript.name == "Planet")
            {
                //yield return StartCoroutine(CameraManager.Instance.changeZoomLevel(CameraManager.MED_ZOOM));
            }
            else if (currCOScript.name == "Moon")
            {
                //yield return StartCoroutine(CameraManager.Instance.changeZoomLevel(CameraManager.CLOSE_ZOOM));
            }
        }
        else
        {
            //StartCoroutine(CameraManager.Instance.changeZoomLevel(CameraManager.FAR_ZOOM));
        }

        yield return new WaitForSeconds(timeBetweenSensorSweeps);
        StartCoroutine(SensorSweep());
    }
}
