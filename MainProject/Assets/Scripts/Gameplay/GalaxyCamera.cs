using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyCamera : Singleton<GalaxyCamera>
{
    #region Fields
    
    //EditorExposed
    [SerializeField]
    private float zoomedOutFarHeight = 75.0f;
    [SerializeField]
    private float zoomedOutHeight = 200.0f;
    [SerializeField]
    private float zoomedInHeight = 100.0f;
    [SerializeField]
    private float camFollowPeriod = 0.1f;

    //Cached references
    private Transform trans;
    private Quaternion initialRot;
    private float initialAngleX;
    private float epsilon = 0.01f;
    private Vector3 target;



    //Events
    public delegate void CameraMoveEvent();
    public event CameraMoveEvent OnCameraMove = new CameraMoveEvent(() => { });

    #endregion Fields


    #region Methods
    public IEnumerator MoveToFocusOn(Transform target)
    {
        //Debug.Log("MoveToFocusOn called...");
        StopCoroutine("FollowMothership");
        StopCoroutine("MoveAndRotate");
        Vector3 targetPos = target.position;
        targetPos.y += zoomedOutFarHeight;
        targetPos.z -= zoomedOutFarHeight / Mathf.Tan(initialAngleX);
        yield return StartCoroutine(MoveAndRotate(targetPos, initialRot, camFollowPeriod));
    }

    public IEnumerator FollowMothership(Transform ship, bool inSystem)
    {
//        Debug.Log("FollowMothershipCalled...");
        StopCoroutine("MoveToFocusOn");
        //StopCoroutine("MoveAndRotate");
        Vector3 targetPos = ship.position;
        if (inSystem)
        {
            targetPos.y += zoomedInHeight;
            targetPos.z -= zoomedInHeight / Mathf.Tan(initialAngleX);
        }
        else
        {
            targetPos.y += zoomedOutHeight;
            targetPos.z -= zoomedOutHeight / Mathf.Tan(initialAngleX);
        }
        yield return StartCoroutine(MoveAndRotate(targetPos, initialRot, camFollowPeriod));
    }

    private IEnumerator MoveAndRotate(Vector3 destination, Quaternion desiredRot, float period)
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

    private void Awake()
    {
        trans = transform;
        initialRot = trans.rotation;
        initialAngleX = Mathf.Deg2Rad * initialRot.eulerAngles.x;
    }
    #endregion Methods

}
