using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class FogOfWar : MonoBehaviour {

    /// <summary>
    /// the player ship's transform
    /// </summary>
    private Transform playerShip;
    private float fogDistance = 10.0f;
    private bool foggy = true;

    private float fadeTimer = 1000.0f;

    /// <summary>
    /// the number of seconds between fog updates
    /// </summary>
    private const float FOG_UPDATE_DELAY = 1.0f;

	void Start () 
    {
        playerShip = GameObject.Find("PlayerShip").transform;
        StartCoroutine(fogTick());
        
	}

    IEnumerator fogTick()
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
            StartCoroutine(FadeFog());
        }


        if (Input.GetKey(KeyCode.F1))
        {
            Application.Quit();
        }
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(fogTick());
    }
	
	void FixedUpdate () 
    {
        
	}

    IEnumerator FadeFog()
    {
        fadeTimer = 1000.0f;
        while(fadeTimer > 0.0f)
        {
            fadeTimer -= 25f;

            Color currColor = renderer.material.color;
            currColor.a = (fadeTimer / 1000.0f) * renderer.material.color.a;

            renderer.material.color = currColor;
        }

        renderer.enabled = false;
        yield return 0;
    }
}
