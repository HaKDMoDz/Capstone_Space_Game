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
    [SerializeField]
    private float cameraFollowPeriod = 0.5f;
    [SerializeField]
    private float cameraMoveToFocusPeriod = 1.0f;
    [SerializeField]
    private float turnDelayFactor = 200.0f; //lower means higher penalty for having high power

    //static vars for easy access
    public static float MovementEpsilon;
    public static float PlayerShipMoveSpeed;
    public static float CameraFollowPeriod;
    public static float CameraMoveToFocusPeriod;
    public static float TurnDelayFactor;

    private void OnEnable()
    {
        MovementEpsilon = movementEpsilon;
        PlayerShipMoveSpeed = playerShipMoveSpeed;
        CameraFollowPeriod = cameraFollowPeriod;
        CameraMoveToFocusPeriod = cameraMoveToFocusPeriod;
        TurnDelayFactor = turnDelayFactor;
    }
}
