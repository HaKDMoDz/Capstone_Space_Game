/*
  CameraDirector.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 16/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDirector : Singleton<CameraDirector>
{
    #region Fields

    //EditorExposed
    [SerializeField]
    private float heightFocus = 200.0f;
    [SerializeField]
    private float heightAiming = 40.0f;
    [SerializeField]
    private float distFocusAiming = 50.0f;
    [SerializeField]
    private Quaternion overheadRotation = Quaternion.Euler(90.0f, 270.0f, 0.0f);
    [SerializeField]
    private float minOverheadHeight = 100.0f;
    [SerializeField]
    private float zoomAboveHeight = 60.0f;
    [SerializeField]
    private float panSpeedToHeightFactor = 0.8f;
    [SerializeField]
    private float zoomSpeed = 10.0f;
    [SerializeField]
    private float minHeight = 120.0f;
    [SerializeField]
    private float maxHeight = 500.0f;
    //[SerializeField]
    //private float orbitSpeed = 30.0f;

    //internal
    private float initialAngleX;
    private Quaternion initialRot;
    private Transform trans;
    private Camera cam;

    public bool Shaking;
    private float ShakeDecay;
    private float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    //Events
    public delegate void CameraMoveEvent();
    public event CameraMoveEvent OnCameraMove = new CameraMoveEvent(() => { });


    #endregion Fields

    #region Methods

    #region PublicMethods

    /// <summary>
    /// Moves the camera to focus on the given target over the specified period. The Camera arrives at a location behind the camera and a predefined height
    /// </summary>
    /// <param name="target"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    public IEnumerator MoveToFocusOn(Transform target, float period)
    {
        Vector3 targetPos = target.position;
        targetPos.y += heightFocus;
        targetPos.z -= heightFocus / Mathf.Tan(initialAngleX);
        yield return StartCoroutine(MoveAndRotate(targetPos, initialRot, period));
    }

    /// <summary>
    /// The camera swoops down to orient itself almost horizationally and aims at the specified target over the specified perdiod. The height above the current focus is predefined.
    /// </summary>
    /// <param name="currentFocus"></param>
    /// <param name="target"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    public IEnumerator AimAtTarget(Transform currentFocus, Transform target, float period)
    {
        Vector3 targetToFocusDir = currentFocus.position - target.position;
        float targetToFocusDist = targetToFocusDir.magnitude;
        targetToFocusDir.Normalize();
        Quaternion desiredCamRotation = Quaternion.LookRotation(-targetToFocusDir);
        targetToFocusDir *= targetToFocusDist + distFocusAiming;
        Vector3 desiredCamPos = target.position + targetToFocusDir + Vector3.up * heightAiming;
        yield return StartCoroutine(MoveAndRotate(desiredCamPos, desiredCamRotation, period));
    }

    /// <summary>
    /// The camera goes into an overhead view that includes the current focus and the target. Will not zoom in lower than minOverheadHeight
    /// </summary>
    /// <param name="currentFocus"></param>
    /// <param name="target"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    public IEnumerator OverheadAimAt(Transform currentFocus, Transform target, float period)
    {
        Vector3 desiredCamPos = (currentFocus.position + target.position) * 0.5f;
        Vector3 directionToTarget = target.position - currentFocus.position;
        float camHeight = directionToTarget.magnitude * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        desiredCamPos.y = camHeight < minOverheadHeight ? minOverheadHeight : camHeight;
        float angle = Vector3.Angle(Vector3.forward, directionToTarget);
        Vector3 perp = Vector3.Cross(Vector3.forward, directionToTarget);
        float dot = Vector3.Dot(perp, currentFocus.up);
        angle = dot > 0.0f ? angle : -angle;
        Quaternion desiredRotation = Quaternion.Euler(overheadRotation.eulerAngles.x, overheadRotation.eulerAngles.y + angle, overheadRotation.eulerAngles.z);
        yield return StartCoroutine(MoveAndRotate(desiredCamPos, desiredRotation, period));
    }

    public IEnumerator ZoomInFromAbove(Transform target, float period)
    {
        Vector3 desiredPos = target.position + Vector3.up * zoomAboveHeight;
        //Quaternion desiredRotation = Quaternion.Euler(overheadRotation.eulerAngles);
        yield return StartCoroutine(MoveAndRotate(desiredPos, trans.rotation, period));
    }

    public void OrbitAroundImmediate(Transform target, float xAngle, float yAngle)
    {
        float distanceToTarget = Vector3.Distance(target.position, trans.position);
        Quaternion rotation = Quaternion.Euler(yAngle, xAngle, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distanceToTarget) + target.position;
        trans.position = position;
        trans.rotation = rotation;
        OnCameraMove();
    }
    public void SetFreeCamera(bool set)
    {
        if (set)
        {
            InputManager.Instance.RegisterKeysHold(KeyboardAction, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
            InputManager.Instance.OnMouseScrollEvent += Zoom;
            StartCoroutine("FreeCamera");
        }
        else
        {
            InputManager.Instance.DeregisterKeysHold(KeyboardAction, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
            InputManager.Instance.OnMouseScrollEvent -= Zoom;
            StopCoroutine("FreeCamera");
        }
    }


    private IEnumerator FreeCamera()
    {
        while (true)
        {
            yield return null;
        }
    }
    public void Pan(Vector2 panVector)
    {
        trans.Translate(panVector * Time.deltaTime);
    }
    public void Zoom(float zoomAmount)
    {
        float newCamHeight = trans.position.y - (zoomAmount * zoomSpeed * Time.deltaTime);
        newCamHeight = Mathf.Clamp(newCamHeight, minHeight, maxHeight);
        trans.SetPositionY(newCamHeight);

    }
    private void KeyboardAction(KeyCode key)
    {
        Vector2 panVector = Vector2.zero;
        float moveSpeed = panSpeedToHeightFactor * trans.position.y;
        if (key == KeyCode.W)
        {
            panVector.y += moveSpeed;
        }
        else if (key == KeyCode.S)
        {
            panVector.y -= moveSpeed;
        }
        if (key == KeyCode.A)
        {
            panVector.x -= moveSpeed;
        }
        else if (key == KeyCode.D)
        {
            panVector.x += moveSpeed;
        }
        Pan(panVector);
    }

    public void DoShake()
    {
        OriginalPos = transform.position;
        OriginalRot = transform.rotation;

        ShakeIntensity = 0.07f;
        ShakeDecay = 0.0005f;
        Shaking = true;
    }
    #endregion PublicMethods

    #region PrivateMethods
    public IEnumerator MoveAndRotate(Vector3 destination, Quaternion desiredRot, float period)
    {
        float time = 0.0f;
        Vector3 startPos = trans.position;
        Quaternion startRot = trans.rotation;
        while (time < 1.0f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            trans.rotation = Quaternion.Slerp(startRot, desiredRot, time);
            time += Time.deltaTime / period;
            OnCameraMove();
            yield return null;

        }
    }
    #region UnityCallbacks
    private void Awake()
    {
        //transform.position = Vector3.zero;
        trans = transform;
        initialRot = Quaternion.Euler(36.0f, 0.0f, 0.0f);
        initialAngleX = Mathf.Deg2Rad * initialRot.eulerAngles.x;
        Shaking = false;
        cam = camera;
    }

    //TODO refactor camera shake with coroutines if performance becomes an issue
    void Update()
    {
        if (ShakeIntensity > 0)
        {
            transform.position = OriginalPos + Random.insideUnitSphere * ShakeIntensity;
            transform.rotation = new Quaternion(OriginalRot.x + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.y + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.z + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f,
                                      OriginalRot.w + Random.Range(-ShakeIntensity, ShakeIntensity) * .2f);

            ShakeIntensity -= ShakeDecay;
        }
        else if (Shaking)
        {
            Shaking = false;
        }
    }

    #endregion UnityCallbacks

    #endregion PrivateMethods

    #endregion Methods
}
