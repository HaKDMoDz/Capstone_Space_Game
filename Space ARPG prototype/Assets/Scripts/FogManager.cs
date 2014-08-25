﻿using UnityEngine;
using System.Collections;

public class FogManager : MonoBehaviour 
{
    GameObject[] Fogs;

    public GameObject fog;

    float startX, startY, currX, currY;

    Transform topLeft, topRight, botLeft, botRight;

	void Start () 
    {
        topLeft = GameObject.Find("TopLeft").transform;
        topRight = GameObject.Find("TopRight").transform;
        botLeft = GameObject.Find("BotLeft").transform;
        botRight = GameObject.Find("BotRight").transform;

        startX = topLeft.position.x;
        startY = topLeft.position.z;

        currX = startX;
        currY = startY;

        Fogs = new GameObject[10000];

        bool done = false;
        int numFog = 0;

        for (int i = 0; i < Fogs.Length; i++)
        {
            if (!done)
            {
                numFog++;

                Fogs[i] = GameObject.Instantiate(fog, new Vector3(currX, 5, currY), Quaternion.identity) as GameObject;

                currX += fog.renderer.bounds.size.x;
                if (currX >= topRight.position.x)
                {
                    currX = startX;
                    currY -= fog.renderer.bounds.size.z;
                }

                if (currY <= botLeft.position.z)
                {
                    done = true;
                }
            }
            
        }
        Debug.Log("numFog: " + numFog);
	}

	void Update () 
    {
	
	}
}
