using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CelestialObjectType {GALAXY, STAR, PLANET, MOON}

public class CelestialObject : MonoBehaviour 
{
    public Transform orbitAroundThis;
    public float rotateSpeed;

    private static int numCelestialObjs;

    public static CelestialObject currSelectedObject;

    public Material glowMaterial;
    public Material blankMaterial;

    public GameObject Atmosphere;

    private Texture texture;

    private int ID;

    public static bool objectSelected;

    public bool disableVisual = false;

    public CelestialObjectType type;

    private Transform playerShipLocation;

    private CelestialObject thisScript;

    public bool selectable = true;
    private bool acceptInput = true;

    public string myName = "";
    public string discoveryDate = "";
    public string composedOf = "";
    public string whyInteresting = "";

    public float myDistanceToShip;

    public int mass;
    public int radius;

	void Start () 
    {
        ID = ++numCelestialObjs;
        myDistanceToShip = 1000000000.0f;
        SystemLog.addMessage("Celestial Object " + ID + ":" + name + " has been initialized");


        playerShipLocation = GameObject.Find("PlayerShip").transform;

        objectSelected = false;

        InputManager.Instance.OnMouseClick += OnMouseClick;

        thisScript = gameObject.GetComponent<CelestialObject>();

        //CheckReadyStatus(); //Debug - used to make sure all components are installed correctly
	}
	
	void Update () 
    {
        Vector3 diffVector = playerShipLocation.position - transform.position;
        myDistanceToShip = diffVector.magnitude;

        /*
         switch (type)
        {
            case CelestialObjectType.STAR:
                disableVisual = false;
                if (myDistanceToShip < medCamRange)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.MED_ZOOM;
                    FatherTime.timeRate = FatherTime.MED_TIME_RATE;
                }

                if (myDistanceToShip > medCamRange * 1.5f)
                {
                    GalaxyCameraDirector.targetZoom = GalaxyCameraDirector.FAR_ZOOM;
                    FatherTime.timeRate = FatherTime.FAR_TIME_RATE;
                }

                break;
            case CelestialObjectType.PLANET:
                if (GalaxyCameraDirector.targetZoom < GalaxyCameraDirector.FAR_ZOOM)
                {
                    disableVisual = false;
                }
                else
                {
                    disableVisual = true;
                }

                if (myDistanceToShip < closeCamRange)
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
                    disableVisual = false;
                }
                else
                {
                    disableVisual = true;
                }
                break;
            default:
                break;

                
        }
         */

        if (objectSelected && selectable)
        {
            //print(currSelectedObject.ID + " vs. " + ID);

            if (currSelectedObject.ID == ID)
            {
                texture = renderer.material.mainTexture;
                renderer.material = glowMaterial;
                renderer.material.mainTexture = texture;
            }
            else
            {
                renderer.material = blankMaterial;

            }
        }

        if (disableVisual)
        {
            renderer.enabled = false;
            if (type == CelestialObjectType.PLANET)
            {
                Atmosphere.renderer.enabled = false;
            }
        }
        else
        {
            renderer.enabled = true;
            if (type == CelestialObjectType.PLANET)
            {
                Atmosphere.renderer.enabled = false;
            }
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

    void OnMouseClick(MouseEventArgs args)
    {
        if (acceptInput)
        {
            if (args.button == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "CelestialObject")
                    {
                        if (hit.collider.GetComponent<CelestialObject>().ID == ID)
                        {
                            currSelectedObject = hit.collider.GetComponent<CelestialObject>();
                            objectSelected = true;
                            SystemLog.addMessage(name + " was selected");
                            DisplayInfo();
                            acceptInput = false;
                            StartCoroutine(WaitBeforeInput(1.0f));
                        }
                    }
                }
            }
        } 
    }

    IEnumerator WaitBeforeInput(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        acceptInput = true;
    }

    void DisplayInfo() 
    {
        Info.EnableMe();
        Info.ResetInfo();
        Info.addMessage("Celestial Object " + ID + ": of type: " + type + " named: " + myName + " has a mass of " + mass.ToString() + ".0 kg and a diameter of " + (radius * 2).ToString() + ".0 m. Discovered in the year " + discoveryDate + " it is composed mainly of " + composedOf + " and has attracted galactic attention because of " + whyInteresting);
        Info.SetInfoToWindow();
    }
}
