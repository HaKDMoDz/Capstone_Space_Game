using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class FogOfWar : MonoBehaviour {

    private Transform playerShip;
    private float fogDistance = 10.0f;
    private bool foggy = true;

    private float fadeTimer = 1000.0f;

    private XmlDocument fogData = new XmlDocument();

	void Start () 
    {
        playerShip = GameObject.Find("PlayerShip").transform;

        
	}
	
	void FixedUpdate () 
    {
        if (foggy)
        {
            if ((playerShip.position - transform.position).magnitude <= fogDistance)
            {
                foggy = false;
            }
        }
        else
        {
            FadeFog();
        }


        if (Input.GetKey(KeyCode.F1))
        {
            OutputFogToXML();
            Application.Quit();
        }
	}

    void FadeFog()
    {
        if (fadeTimer > 0.0f)
        {
            fadeTimer -= 25f;

            Color currColor = renderer.material.color;
            currColor.a = (fadeTimer / 1000.0f);

            renderer.material.color = currColor;
        }
        else
        {
            renderer.enabled = false;
        }
    }

    void OutputFogToXML()
    {

    }

    void InputFogFromXML()
    {
        
    }
}
