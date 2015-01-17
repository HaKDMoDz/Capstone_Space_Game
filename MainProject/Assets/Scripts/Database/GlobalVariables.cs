using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVariables : ScriptableObject 
{
    [SerializeField]
    private float movementEpsilon = 0.2f;
    public static float MovementEpsilon = 0.2f;	

    private void OnEnable()
    {

    }
}
