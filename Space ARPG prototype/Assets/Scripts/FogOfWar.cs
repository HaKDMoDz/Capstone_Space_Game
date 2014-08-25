using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {

    private Transform playerShip;
    private float fogDistance = 10.0f;
    private bool foggy = true;


	// Use this for initialization
	void Start () 
    {
        playerShip = GameObject.Find("PlayerShip").transform;


	}
	
	// Update is called once per frame
	void Update () 
    {
        if (foggy)
        {
            if ((playerShip.position - transform.position).magnitude <= fogDistance)
            {
                renderer.enabled = false;
                foggy = false;
            }
        }
        
	}
}
