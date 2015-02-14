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
    //[SerializeField]
    //private float orbitSpeed = 30.0f;
    
    //internal
    private float initialAngleX;
    private Quaternion initialRot;
    private Transform trans;

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

    public void OrbitAroundImmediate(Transform target, float xAngle, float yAngle)
    {
        float distanceToTarget = Vector3.Distance(target.position, trans.position);
        Quaternion rotation = Quaternion.Euler(yAngle, xAngle, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distanceToTarget) + target.position;
        trans.position = position;
        trans.rotation = rotation;
        OnCameraMove();
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
    private IEnumerator MoveAndRotate(Vector3 destination, Quaternion desiredRot, float period)
    {
        float time = 0.0f;
        Vector3 startPos = trans.position;
        Quaternion startRot = trans.rotation;
        while(time<1.0f)
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
        trans = transform;
        initialRot = trans.rotation;
        initialAngleX = Mathf.Deg2Rad * initialRot.eulerAngles.x;
        Shaking = false;
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
