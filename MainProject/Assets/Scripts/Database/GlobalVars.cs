using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : ScriptableObject 
{
    //Editor Exposed
    [SerializeField]
    private float lerpDistanceEpsilon = 0.2f;
    [SerializeField]
    private float shipMoveSpeed = 3.0f;
    [SerializeField]
    private float cameraFollowPeriod = 0.5f;
    [SerializeField]
    private float cameraMoveToFocusPeriod = 1.0f;
    [SerializeField]
    private float cameraAimAtPeriod = 1.0f;
    [SerializeField]
    private float turnDelayFactor = 200.0f; //lower means higher penalty for having high power

    //static vars for easy access
    public static float LerpDistanceEpsilon { get; private set; }
    public static float ShipMoveSpeed { get; private set; }
    public static float CameraFollowPeriod { get; private set; }
    public static float CameraAimAtPeriod { get; private set; }
    public static float CameraMoveToFocusPeriod { get; private set; }
    public static float TurnDelayFactor { get; private set; }

    private void OnEnable()
    {
        LerpDistanceEpsilon = lerpDistanceEpsilon;
        ShipMoveSpeed = shipMoveSpeed;
        CameraFollowPeriod = cameraFollowPeriod;
        CameraMoveToFocusPeriod = cameraMoveToFocusPeriod;
        CameraAimAtPeriod = cameraAimAtPeriod;
        TurnDelayFactor = turnDelayFactor;
    }
}
