using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDirector : Singleton<CameraDirector>
{
    #region Fields
    #region EditorExposed
    
    [SerializeField]
    private float heightFocus = 200.0f;
    [SerializeField]
    private float heightAiming = 40.0f;
    [SerializeField]
    private float distFocusAiming = 50.0f;
    [SerializeField]
    private float orbitSpeed = 30.0f;
    #endregion EditorExposed

    #region Internal
    private float initialAngleX;
    private Quaternion initialRot;
    private Transform trans;
    #endregion Internal


    public bool Shaking;
    private float ShakeDecay;
    private float ShakeIntensity;
    private Vector3 OriginalPos;
    private Quaternion OriginalRot;

    #region Events
    public delegate void CameraMoveEvent();
    public event CameraMoveEvent OnCameraMove = new CameraMoveEvent(() => { });

    #endregion Events

    #endregion Fields

    #region Methods

    #region PublicMethods

    public IEnumerator MoveToFocusOn(Transform target, float period)
    {
        Vector3 targetPos = target.position;
        targetPos.y += heightFocus;
        targetPos.z -= heightFocus / Mathf.Tan(initialAngleX);
        yield return StartCoroutine(MoveAndRotate(targetPos, initialRot, period));
    }
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
