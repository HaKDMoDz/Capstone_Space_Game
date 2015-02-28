using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SystemNamer : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        gameObject.GetComponent<Text>().text = transform.parent.parent.name;
	}
}
