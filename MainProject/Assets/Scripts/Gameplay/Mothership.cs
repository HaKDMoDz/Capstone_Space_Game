using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    #region Fields

    //EditorExposed
    [SerializeField]
    private SpaceGround spaceGround;
    [SerializeField]
    private float moveSpeed = 1.0f;


    //cached
    private Transform trans;

    private Vector3 destination;
    private bool moving = false;
    #endregion Fields

    #region Methods

    #region PrivateMethods

    private IEnumerator Move()
    {
        Vector3 moveDir;
        trans.LookAt(destination);
        moving = true;
        do
        {
            trans.LookAt(destination);
            moveDir = destination - trans.position;
            trans.position = Vector3.Lerp(trans.position, destination, moveSpeed * Time.deltaTime);
            StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraFollowPeriod * 0.1f));
            yield return null;

        } while (Vector3.SqrMagnitude(moveDir) > GlobalVars.LerpDistanceEpsilon * GlobalVars.LerpDistanceEpsilon);
        
        moving = false;
    }

    #region UnityCallbacks
    private void Awake()
    {
        trans = transform;
    }
    private void Start()
    {
        spaceGround.OnGroundRightClick += OnGroundClick;
        StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraFollowPeriod ));
    }

    #endregion UnityCallbacks

    #region InternalCallbacks
    
    void OnGroundClick(Vector3 worldPosition)
    {
        destination = worldPosition;
        if (!moving)
        {
            StartCoroutine(Move());
        }
    }
    
    #endregion InternalCallbacks
    
    #endregion PrivateMethods

    #endregion Methods
}
