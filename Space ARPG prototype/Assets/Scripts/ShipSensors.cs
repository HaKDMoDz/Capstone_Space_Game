using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum shipSensorStates { OUTER_SPACE, STELLAR_ORBIT, PLANETARY_ORBIT}

public class ShipSensors : MonoBehaviour 
{
    private GameObject[] allStellarObjects;
    private List<GameObject> objectsNearMe;
    private GameObject closestObjectToMe;
    private GameObject orbitting;

    public float sensorRange;

    private Camera cam;
    private float medCamRange = 50.0f;
    private float closeCamRange = 20.0f;
    private float timeBetweenSensorSweeps = 2.0f;
    private float timeUntilSensorSweep = 0.0f;

    private bool inASystem = false;
	// Use this for initialization
	void Start () 
    {
        allStellarObjects = new GameObject[0];
        objectsNearMe = new List<GameObject>();
        closestObjectToMe = new GameObject();
        closestObjectToMe.SetActive(false);

        allStellarObjects = GameObject.FindGameObjectsWithTag("CelestialObject");

        cam = Camera.main;

        orbitting = null;
	}
	
	// Update is called once per frame
	void Update () 
    {
        CelestialObject currCOScript;
        for (int i = objectsNearMe.Count - 1; i > 0; i--)
		{
            currCOScript = objectsNearMe[i].GetComponent<CelestialObject>();
            if (currCOScript.myDistanceToShip > sensorRange)
            {
                objectsNearMe.Remove(objectsNearMe[i]);
            }
	    }

        //sensor sweep
        if (timeUntilSensorSweep <= 0.000001f)
        {
            timeUntilSensorSweep = timeBetweenSensorSweeps;
            foreach (GameObject gObject in allStellarObjects)
            {
                float distance = (gObject.transform.position - transform.position).magnitude;
                gObject.GetComponent<CelestialObject>().myDistanceToShip = distance;

                if (distance < sensorRange)
                {
                    objectsNearMe.Add(gObject);
                    if (!closestObjectToMe.activeInHierarchy)
                    {
                        closestObjectToMe = gObject;
                    }
                    else
                    {
                        if (distance < closestObjectToMe.GetComponent<CelestialObject>().myDistanceToShip)
                        {
                            closestObjectToMe = gObject;

                            //SystemLog.addMessage("tick");
                            //SystemLog.addMessage(closestObjectToMe.GetComponent<CelestialObject>().myName);
                        }
                    }
                }
            }
        }
        else
        {
            timeUntilSensorSweep -= Time.deltaTime;
        }

        currCOScript = closestObjectToMe.GetComponent<CelestialObject>();

        if (currCOScript.myDistanceToShip < medCamRange)
        {
            GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
            FatherTime.timeRate = FatherTime.MED_TIME_RATE;
        }
        else if (currCOScript.myDistanceToShip > medCamRange * 1.5f)
        {
            GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.FAR_ZOOM;
            FatherTime.timeRate = FatherTime.FAR_TIME_RATE;
        }

        if ((currCOScript.myDistanceToShip < closeCamRange) && (currCOScript.type == CelestialObjectType.PLANET))
        {

            GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.CLOSE_ZOOM;
            FatherTime.timeRate = FatherTime.CLOSE_TIME_RATE;
        }
        else if ((currCOScript.myDistanceToShip > medCamRange) && (currCOScript.type == CelestialObjectType.MOON || currCOScript.type == CelestialObjectType.PLANET))
        {
            GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
            FatherTime.timeRate = FatherTime.MED_TIME_RATE;
        }
	}
}
