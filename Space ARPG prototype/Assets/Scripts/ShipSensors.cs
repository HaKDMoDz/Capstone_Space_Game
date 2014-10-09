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

        yield return new WaitForSeconds(timeBetweenSensorSweeps);
        StartCoroutine(SensorSweep());
    }
}
