using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDirector : SingletonComponent<CameraDirector>
{

    [SerializeField]
    Transform sceneStartPos;
    [SerializeField]
    float movementEpsilon = 0.2f;
    [SerializeField]
    float heightAboveFocusTarget = 200f;
    [SerializeField]
    float heightAboveFocusWhileAiming = 40f;
    [SerializeField]
    float distToFocusWhileAiming = 50f;
    [SerializeField]
    float cameraOrbitSpeed = 30f;

    Quaternion initialRot;
    float initialAngleX;
    Transform trans;

    public delegate void CameraMoved();
    public event CameraMoved OnCamerMove = new CameraMoved(delegate(){});

    protected override void Awake()
    {
        base.Awake();
        trans = transform;
        initialAngleX = Mathf.Deg2Rad * trans.rotation.eulerAngles.x;
        initialRot = trans.rotation;
    }
    public IEnumerator MoveTo(Vector3 destination, float period)
    {
        float time = 0f;
        Vector3 startPos = trans.position;
        while (time < 1f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            time += Time.deltaTime / period;
            OnCamerMove();
            yield return null;
        }
    }
    public IEnumerator MoveAndRotateTo(Vector3 destination, Quaternion desiredRotation, float period)
    {
        float time = 0f;
        Vector3 startPos = trans.position;
        Quaternion startRot = trans.rotation;
        while (time < 1f)
        {
            trans.position = Vector3.Lerp(startPos, destination, time);
            trans.rotation = Quaternion.Slerp(startRot, desiredRotation, time);
            time += Time.deltaTime / period;
            OnCamerMove();
            yield return null;
        }
    }

    public IEnumerator LerpTo(Vector3 destination, float speed)
    {
        Vector3 destDir = destination-trans.position;
        while(Vector3.SqrMagnitude(destDir)>movementEpsilon*movementEpsilon)
        {
            trans.position = Vector3.Lerp(trans.position, destination, speed * Time.deltaTime);
            yield return null;
            OnCamerMove();
        }
    }

    public IEnumerator FocusOn(Transform target, float period)
    {
        Vector3 targetPos = target.position;
        targetPos.y += heightAboveFocusTarget;
        targetPos.z -= heightAboveFocusTarget / Mathf.Tan(initialAngleX);
        yield return StartCoroutine(MoveAndRotateTo(targetPos, initialRot, period));
        //yield return StartCoroutine(MoveTo(targetPos, period));
    }
    public IEnumerator AimAtTarget(Transform currentFocus, Transform target, float period)
    {

        Vector3 targetToFocusDir = currentFocus.position - target.position;
        float targetToFocusDist = targetToFocusDir.magnitude;
        targetToFocusDir.Normalize();
        Quaternion desiredCamRotation = Quaternion.LookRotation(-targetToFocusDir);
        targetToFocusDir *= targetToFocusDist + distToFocusWhileAiming;
        Vector3 desiredCamPos = target.position + targetToFocusDir + Vector3.up*heightAboveFocusWhileAiming;

        yield return StartCoroutine(MoveAndRotateTo(desiredCamPos, desiredCamRotation, period));
    }
    

    public void OrbitAroundImmediate(Transform target, float xAngle, float yAngle)
    {
        trans.RotateAround(target.position, Vector3.up, xAngle*cameraOrbitSpeed*Time.deltaTime);
        trans.RotateAround(target.position, Vector3.right, yAngle*cameraOrbitSpeed*Time.deltaTime);
        OnCamerMove();
    }

}
