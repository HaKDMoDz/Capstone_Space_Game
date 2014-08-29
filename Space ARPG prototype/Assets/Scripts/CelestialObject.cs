using UnityEngine;
using System.Collections;

public enum CelestialObjectType {GALAXY, STAR, PLANET, MOON}

public class CelestialObject : MonoBehaviour 
{
    public static bool inASystem = false;
    public Transform orbitAroundThis;
    public float rotateSpeed;

    private static int numCelestialObjs;
    private int ID;

    public bool disableVisual = false;

    public CelestialObjectType type;

    private Camera cam;

    private float medCamRange = 50.0f;
    private float closeCamRange = 20.0f;

    private Transform playerShipLocation;
	void Start () 
    {
        ID = ++numCelestialObjs;

        name = "Celestial Object " + ID;

        cam = Camera.main;

        playerShipLocation = GameObject.Find("PlayerShip").transform;

        CheckReadyStatus();
	}
	
	void Update () 
    {
        switch (type)
        {
            case CelestialObjectType.STAR:
                disableVisual = false;
                
                if (Vector3.SqrMagnitude(playerShipLocation.position - transform.position) < medCamRange * medCamRange)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
                    FatherTime.timeRate = FatherTime.MED_TIME_RATE;
                    inASystem = true;
                }

                if (Vector3.SqrMagnitude(playerShipLocation.position - transform.position) > medCamRange * medCamRange * 1.5f)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.FAR_ZOOM;
                    FatherTime.timeRate = FatherTime.FAR_TIME_RATE;
                    inASystem = false;
                }
                    
                /*
                    else
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.FAR_ZOOM;
                    FatherTime.timeRate = FatherTime.FAR_TIME_RATE;
                    inASystem = false;
                }
                    */

                break;
            case CelestialObjectType.PLANET:
                if (Mathf.Abs(GalaxyCameraDirector.FAR_ZOOM - cam.orthographicSize) < 0.9f)
                {
                    disableVisual = true;
                }
                else 
                {
                    disableVisual = false;
                }

                if (inASystem)
                {
                    if (Vector3.SqrMagnitude(playerShipLocation.position - transform.position) < closeCamRange * closeCamRange)
                    {
                        GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.CLOSE_ZOOM;
                        FatherTime.timeRate = FatherTime.CLOSE_TIME_RATE;
                    }
                    else
                    {
                        GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
                        FatherTime.timeRate = FatherTime.MED_TIME_RATE;
                    }
                }
                break;
            case CelestialObjectType.MOON:
                if (Mathf.Abs(GalaxyCameraDirector.FAR_ZOOM - cam.orthographicSize) < 0.9f || Mathf.Abs(GalaxyCameraDirector.MED_ZOOM - cam.orthographicSize) < 0.9f)
                {
                    disableVisual = true;
                }
                else
                {
                    disableVisual = false;
                }
                break;
            default:
                break;
        }

        if (disableVisual)
        {
            renderer.enabled = false;
        }
        else
        {
            renderer.enabled = true;
        }

        transform.RotateAround(orbitAroundThis.position, Vector3.up, rotateSpeed * Time.deltaTime * FatherTime.timeRate);
        /*
         * the following is debug code. replace the previous line of code with this to ensure components are correctly installed
         * 
        if (CheckReadyStatus())
        {
            transform.RotateAround(orbitAroundThis.position, Vector3.up, rotateSpeed * Time.deltaTime * FatherTime.timeRate);
            //replace with proper celestial rotation code from ThrusterPrototype's galaxy code
        }
         */
	}

    //this is a debug method used to make sure all components are installed correctly
    private bool CheckReadyStatus()
    {
        bool ready = false;

        if (orbitAroundThis && rotateSpeed > 0.0f)
        {
            ready = true;
        }
        else if (!orbitAroundThis)
        {
            Debug.LogError("CelestialBody " + ID + " has nothing to rotate around. please set in editor");
        }
        else if (!(rotateSpeed > 0.0f))
        {
            Debug.LogError("CelestialBody " + ID + " has no rotation speed. please set in editor");
        }

        return ready;
    }
}
