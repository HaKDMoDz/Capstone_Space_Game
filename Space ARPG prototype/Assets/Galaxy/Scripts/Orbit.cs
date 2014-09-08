using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour 
{
    public GameObject GalaxyCore;
    public 
	// Use this for initialization
	void Start () 
    {
        GalaxyCore = GameObject.Find("GalaxyCore") as GameObject;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(GalaxyCore)
        {
           // transform.RotateAround(GalaxyCore.transform.position, Vector3.up, 0.5f);
        }
	}
}
