using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipSensors : MonoBehaviour 
{
    private GameObject[] allStellarObjects;
    private List<GameObject> objectsNearMe;
    private GameObject closestObjectToMe;

    public float sensorRange;

    private Camera cam;
    private float medCamRange = 40.0f;
    private float closeCamRange = 10.0f;

	// Use this for initialization
	void Start () 
    {
        allStellarObjects = new GameObject[0];
        objectsNearMe = new List<GameObject>();
        closestObjectToMe = new GameObject();
        closestObjectToMe.SetActive(false);

        allStellarObjects = GameObject.FindGameObjectsWithTag("CelestialObject");

        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
    {
        for (int i = objectsNearMe.Count - 1; i > 0; i--)
			{
                if (objectsNearMe[i].GetComponent<CelestialObject>().myDistanceToShip > sensorRange)
                {
                    objectsNearMe.Remove(objectsNearMe[i]);
                }
	    }

        //sensor sweep
        foreach (GameObject gObject in allStellarObjects)
        {
            float distance = (gObject.transform.position - transform.position).magnitude;
            
            if ( distance < sensorRange)
            {
                objectsNearMe.Add(gObject);
                if (!closestObjectToMe.activeInHierarchy)
                {
                    closestObjectToMe = gObject;
                }
                else
                {
                    if (gObject.GetComponent<CelestialObject>().myDistanceToShip < closestObjectToMe.GetComponent<CelestialObject>().myDistanceToShip)
                    {
                        closestObjectToMe = gObject;
                        SystemLog.addMessage("tick");
                        SystemLog.addMessage(closestObjectToMe.GetComponent<CelestialObject>().myName);
                    }
                }
                
            }
        }

        CelestialObject closestObject = closestObjectToMe.GetComponent<CelestialObject>();
        switch (closestObject.type)
        {
            case CelestialObjectType.STAR:
                closestObject.disableVisual = false;

                if (closestObject.myDistanceToShip < medCamRange)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
                    FatherTime.timeRate = FatherTime.MED_TIME_RATE;
                }

                if (closestObject.myDistanceToShip > medCamRange * 1.5f)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.FAR_ZOOM;
                    FatherTime.timeRate = FatherTime.FAR_TIME_RATE;
                }

                break;
            case CelestialObjectType.PLANET:
                if (GalaxyCameraDirector.targetZoom < GalaxyCameraDirector.FAR_ZOOM)
                {
                    closestObject.disableVisual = false;
                }
                else
                {
                    closestObject.disableVisual = true;
                }

                if (closestObject.myDistanceToShip < closeCamRange)
                {

                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.CLOSE_ZOOM;
                    FatherTime.timeRate = FatherTime.CLOSE_TIME_RATE;
                }
                break;
            case CelestialObjectType.MOON:

                //There is a bug here if you set the if statement to GalaxyCameraDirector.targetZoom instead of cam.orthographicSize it doesnt show moons

                if (cam.orthographicSize < GalaxyCameraDirector.MED_ZOOM)
                //if (GalaxyCameraDirector.targetZoom < GalaxyCameraDirector.CLOSE_ZOOM)
                {
                    closestObject.disableVisual = false;
                }
                else
                {
                    closestObject.disableVisual = true;
                }
                break;
            default:
                break;


        }


	}
}
