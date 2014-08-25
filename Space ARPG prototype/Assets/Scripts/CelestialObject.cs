using UnityEngine;
using System.Collections;

public class CelestialObject : MonoBehaviour 
{

    public Transform orbitAroundThis;
    public float rotateSpeed;

    private static int numCelestialObjs;
    private int ID;

	void Start () 
    {
        ID = ++numCelestialObjs;

        name = "Celestial Object " + ID;

        CheckReadyStatus();
	}
	
	void Update () 
    {

        if (CheckReadyStatus())
        {
            transform.RotateAround(orbitAroundThis.position, Vector3.up, rotateSpeed * Time.deltaTime);

        }
	}

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
