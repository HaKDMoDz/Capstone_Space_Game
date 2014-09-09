using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDirector : SingletonComponent<CameraDirector>
//public class CameraDirector : MonoBehaviour
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
            trans.rotation = Quaternion.Lerp(startRot, desiredRotation, time);
            time += Time.deltaTime / period;
            OnCamerMove();
            yield return null;
        }
    }

    public IEnumerator LerpTo(Vector3 destination, float period)
    {
        yield return null;
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


        //Vector2 A, B, C, D, E, F, T;
        //A = new Vector2(currentFocus.position.x, currentFocus.position.z);
        //B = new Vector2(trans.position.x, trans.position.z);
        //T = new Vector2(target.position.x, target.position.z);
        //float focusToCamDist_AB = Vector2.Distance(A, B);
        //C = new Vector2(trans.position.x, trans.position.z);
        //C.x = (A.x - T.x) * (B.y - T.y) / (A.y - T.y) + T.x;
        //float BC = Vector2.Distance(B, C);
        //float AC = Mathf.Sqrt(focusToCamDist_AB * focusToCamDist_AB + BC * BC);
        //float DC = AC - focusToCamDist_AB;
        //float Theta = Mathf.Atan2(focusToCamDist_AB, BC);
        //float EC = Mathf.Cos(Theta) * DC;
        //float DE = Mathf.Tan(Theta) * EC;
        //float EB = BC - EC;
        //Debug.Log("BC: " + BC);
        //Debug.Log("EC: " + EC);

        //Debug.Log("B " + B);
        //Debug.Log("EB " + EB);
        //D = new Vector3(B.x + EB, heightAboveFocusWhileAiming, B.y + DE);
        //Vector3 targetPos = D;

        //float Alpha = 180 - Mathf.Rad2Deg * Theta;

        //Debug.Log(targetPos);

        ////Vector3 targetPos = currentFocus.position;
        ////targetPos.y += heightAboveFocusWhileAiming;
        ////targetPos.z -= heightAboveFocusWhileAiming / Mathf.Tan(initialAngleX);
        //Quaternion desiredRotation = trans.rotation;
        ////desiredRotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y+Alpha, desiredRotation.eulerAngles.z);
        //desiredRotation = Quaternion.Euler(0, Alpha, desiredRotation.eulerAngles.z);

        //Vector3 targetToFocus = target.position - currentFocus.position;
        //Vector3 camPosOnFocusPlane = trans.position;
        //camPosOnFocusPlane.y = currentFocus.position.y;
        //Vector3 camToFocus = currentFocus.position - camPosOnFocusPlane;
        //float angle = Vector3.Angle(targetToFocus, camToFocus);
        //Debug.Log("Angle: " + angle);
        //Vector3 relativePoint = transform.InverseTransformPoint(target.position);
        //if (relativePoint.x < 0.0)
        //{
        //    print("Object is to the left");
        //    angle *= -1f;
        //}
        ////else if (relativePoint.x > 0.0)
        ////    print("Object is to the right");
        ////else
        ////    print("Object is directly ahead");


        //OrbitAroundImmediate(currentFocus, angle, 0);


        

        //yield return null;

    }
    

    public void OrbitAroundImmediate(Transform target, float xAngle, float yAngle)
    {
        trans.RotateAround(target.position, Vector3.up, xAngle*cameraOrbitSpeed*Time.deltaTime);
        trans.RotateAround(target.position, Vector3.right, yAngle*cameraOrbitSpeed*Time.deltaTime);
        OnCamerMove();
    }

}
