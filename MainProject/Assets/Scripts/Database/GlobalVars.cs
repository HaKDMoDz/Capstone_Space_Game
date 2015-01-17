using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : ScriptableObject 
{
    //Editor Exposed
    [SerializeField]
    private float movementEpsilon = 0.2f;
    [SerializeField]
    private float playerShipMoveSpeed = 3.0f;
    //static vars for easy access
    public static float MovementEpsilon;
    public static float PlayerShipMoveSpeed;

    private void OnEnable()
    {
        MovementEpsilon = movementEpsilon;
        PlayerShipMoveSpeed = playerShipMoveSpeed;
    }
}
