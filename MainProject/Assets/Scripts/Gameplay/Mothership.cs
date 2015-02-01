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
    private bool inSystem = false;
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
            StartCoroutine(GalaxyCamera.Instance.FollowMothership(trans,inSystem));
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
        spaceGround.OnGroundClick += OnGroundClick;
        spaceGround.OnGroundHold += OnGroundClick;
        StartCoroutine(GalaxyCamera.Instance.MoveToFocusOn(trans));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.SolarSystemTag)
        {
            inSystem = true;
            #if FULL_DEBUG
            Debug.Log("In System: " + inSystem);
            #endif
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == TagsAndLayers.SolarSystemTag)
        {
            inSystem = false;
            #if FULL_DEBUG
            Debug.Log("In System: " + inSystem);
            #endif
        }
    }
    #endregion UnityCallbacks

    #region InternalCallbacks
    
    void OnGroundClick(Vector3 worldPosition)
    {
        Debug.Log("mothership click");
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
