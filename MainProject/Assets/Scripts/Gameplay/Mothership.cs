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
    private float moveSpeed = 0.5f;
    [SerializeField]
    private float orbitSpeed = 0.01f;
    private Quaternion orbitalRotation;
    private bool orbitting;

    private float angle = 0.0f;


    //cached
    private Transform trans;

    private Vector3 destination;
    private Vector3 orbitDestination;
    private bool moving = false;
    private bool inSystem = false;
    #endregion Fields

    #region Methods

    #region PrivateMethods

    private IEnumerator Move()
    {
        if (!orbitting)
        {
            Vector3 moveDir;
            trans.LookAt(destination);
            moving = true;
            do
            {
                trans.LookAt(destination);
                moveDir = destination - trans.position;
                trans.position = Vector3.Lerp(trans.position, destination, moveSpeed * Time.deltaTime);
                StartCoroutine(GalaxyCamera.Instance.FollowMothership(trans, inSystem));
                yield return null;

            } while (Vector3.SqrMagnitude(moveDir) > GlobalVars.LerpDistanceEpsilon * GlobalVars.LerpDistanceEpsilon);

            moving = false;
        }
        yield return null;
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
        if (other.tag == TagsAndLayers.PlanetTag)
        {
            Debug.Log("entering planter orbit");
            //TODO: add code for setting initial angle
            float deltaZ = other.transform.position.z - transform.position.z;
            float deltaX = other.transform.position.x - transform.position.x;
            angle = Mathf.Atan(deltaZ/deltaX) * 180.0f / Mathf.PI;
            orbitalRotation = other.transform.rotation;
            orbitting = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == TagsAndLayers.PlanetTag && orbitting)
        {
            //Debug.Log("<<<" + other.name + ">>>");

            angle = (angle + orbitSpeed) % 360.0f;
            Debug.Log(angle);

            transform.position = PointOnCircle(other.GetComponent<SphereCollider>().radius, angle, other.transform.position);
            destination = PointOnCircle(other.GetComponent<SphereCollider>().radius, (angle + 2.0f)% 360.0f, other.transform.position);

            transform.rotation = orbitalRotation;
            moving = false;
            
        }
    }

    //helper
    private Vector3 PointOnCircle(float radius, float angleInDegrees, Vector3 origin)
    {
        // Convert from degrees to radians via multiplication by PI/180        
        float x = (float)(radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180F)) + origin.x;
        float z = (float)(radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180F)) + origin.z;

        return new Vector3(x, origin.y, z);
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
        if (other.tag == TagsAndLayers.PlanetTag)
        {
            orbitting = false;
        }
    }
    #endregion UnityCallbacks

    #region InternalCallbacks
    
    void OnGroundClick(Vector3 worldPosition)
    {
        Debug.Log("mothership click");
        destination = worldPosition;
        orbitting = false;
        if (!moving)
        {
            StartCoroutine(Move());
        }
    }

    #endregion InternalCallbacks
    
    #endregion PrivateMethods

    #endregion Methods
}
