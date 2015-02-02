using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemObject : MonoBehaviour 
{
    private void OnMouseDown()
    {
        Debug.Log("clicked");
        GameObject.Find("Mothership").GetComponent<Mothership>().Orbiting = true;
    }
}
