using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class test : MonoBehaviour 
{
    void Start()
    {
        Invoke("Anim", 2.0f);
    }
    void Anim()
    {
        GetComponent<Animation>().Play("Take 001");
        
    }

}
