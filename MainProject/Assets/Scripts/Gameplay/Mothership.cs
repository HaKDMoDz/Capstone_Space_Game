using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mothership : MonoBehaviour
{
    #region Fields

    public enum MovementMode { Lerp, Translate}
    [SerializeField]
    private MovementMode movementMode;
    //EditorExposed
    [SerializeField]
    private SpaceGround spaceGround;
    [SerializeField]
    private float moveSpeed = 0.5f;
    [SerializeField]
    private float orbitSpeed = 0.01f;
    //private Quaternion orbitalRotation;
    private bool orbiting;

    private float angle = 0.0f;

    public bool Orbiting { get { return orbiting; } set { orbiting = value; } }

    public int orbitID = -1;

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
        Debug.Log("movement: called");
        if (!orbiting)
        {
            Debug.Log("movement: not orbiting");
            Vector3 moveDir;
            moving = true;
            moveDir = destination - trans.position;

            if (movementMode == MovementMode.Translate)
            {
                Debug.Log("movement: translate phase");
                while (moveDir.magnitude > moveSpeed * Time.deltaTime)
                {
                    Debug.Log("movement: translate phase: moving");
                    Vector3 moveDirNorm = moveDir.normalized;
                    trans.LookAt(destination);
                    trans.position += moveDirNorm * moveSpeed * Time.deltaTime;
                    moveDir = destination - trans.position;
                    //StartCoroutine(GalaxyCamera.Instance.FollowMothership(trans, inSystem));
                    GalaxyCamera.Instance.targetMothership();
                    yield return null;

                }
            }
            while (moveDir.magnitude > GlobalVars.LerpDistanceEpsilon)
            {
                Debug.Log("movement: lerp phase");
                trans.LookAt(destination);
                trans.position = Vector3.Lerp(trans.position, destination, moveSpeed*.01f * Time.deltaTime);
                //StartCoroutine(GalaxyCamera.Instance.FollowMothership(trans, inSystem));
                GalaxyCamera.Instance.targetMothership();
                moveDir = destination - trans.position;
                yield return null;
            }
            
            moving = false;
        }
        yield return null;
    }

    private IEnumerator Orbit()
    {
        Debug.Log("orbit: called");
        Vector3 moveDir;
        moving = true;
        moveDir = destination - trans.position;

        if (movementMode == MovementMode.Translate)
        {
            //Debug.Log("orbit: translate mode");
            while (moveDir.magnitude > moveSpeed * Time.deltaTime)
            {
                //Debug.Log("orbit: speed under max");
                Vector3 moveDirNorm = moveDir.normalized;
                trans.LookAt(destination);
                trans.position += moveDirNorm * moveSpeed * Time.deltaTime;
                moveDir = destination - trans.position;
                yield return null;

            }
        }
        while (moveDir.magnitude > GlobalVars.LerpDistanceEpsilon)
        {
            //Debug.Log("orbit: not at destination yet");
            trans.LookAt(destination);
            trans.position = Vector3.Lerp(trans.position, destination, moveSpeed * .01f * Time.deltaTime);
            moveDir = destination - trans.position;
            yield return null;
        }
    }

    #region UnityCallbacks
    private void Awake()
    {
        trans = transform;
    }
    private void Start()
    {
        GameController.Instance.OnQuit += SaveData;
        GameController.Instance.OnPreSceneChange += (SceneChangeArgs)=>SaveData();
        spaceGround.OnGroundClick += OnGroundClick;
        spaceGround.OnGroundHold += OnGroundClick;
        GalaxyCamera.Instance.targetMothership();
        GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SPACE_ZOOM);

        if (GameController.Instance.GameData.galaxyMapData.position != Vector3.zero)
        {
            transform.position = GameController.Instance.GameData.galaxyMapData.position;
        }

    }
    private void SaveData()
    {
        GameController.Instance.GameData.galaxyMapData.position = trans.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.SolarSystemTag)
        {
            inSystem = true;
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SYSTEM_ZOOM);
            #if FULL_DEBUG
            //Debug.Log("In System: " + inSystem);
            #endif
        }
        if (other.tag == TagsAndLayers.PlanetTag)
        {
            //Debug.Log("entering planetary orbit");
            
            Transform otherTrans = other.transform;
            float deltaZ = otherTrans.position.z - trans.position.z;
            float deltaX = otherTrans.position.x - trans.position.x;
            angle = (180.0f + ((Mathf.Atan2(deltaZ,deltaX) * 180.0f) / Mathf.PI))%360.0f;
            //orbitalRotation = otherTrans.rotation;
            orbiting = true;
            orbitID = otherTrans.gameObject.GetComponent<Planet_Dialogue>().ID;
            //Debug.Log("OrbitID: " + orbitID);
            StartCoroutine(otherTrans.gameObject.GetComponent<PlanetUIManager>().enableUIRing());
            //StopCoroutine(Move());
            GalaxyCamera.Instance.targetPlanet(otherTrans);
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.PLANET_ZOOM);
            //StopAllCoroutines();
            StopCoroutine(Move());
            StartCoroutine(Orbit());

            //StartCoroutine(GalaxyCamera.Instance.FocusOnPlanet(otherTrans));


        }
    }
    private void OnTriggerStay(Collider other)
    {
        StopCoroutine(Move());
        if (other.tag == TagsAndLayers.PlanetTag && orbiting)
        {
            //Debug.Log("<<<" + other.name + ">>>");
           // Debug.Log("still in Planetary Orbit...");
            
            angle = (angle + orbitSpeed) % 360.0f;
            //Debug.Log(angle);
            Transform otherTrans = other.transform;
            trans.position = PointOnCircle(((SphereCollider)other).radius - 10.0f, angle, otherTrans.position);
            destination = PointOnCircle(((SphereCollider)other).radius - 10.0f, (angle + 2.0f) % 360.0f, otherTrans.position);
            trans.LookAt(destination);
            moving = false;
            //GalaxyCamera.Instance.targetPlanet(otherTrans);
            //GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.PLANET_ZOOM);

           
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
            GalaxyCamera.Instance.targetMothership();
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SPACE_ZOOM);
#if FULL_DEBUG
            //Debug.Log("In System: " + inSystem);
#endif
        }
        if (other.tag == TagsAndLayers.PlanetTag && orbitID == other.gameObject.GetComponent<Planet_Dialogue>().ID)
        {
            orbiting = false;
            //moving = true;
            StopCoroutine(Orbit());
            //StartCoroutine(Move());
            StartCoroutine(other.gameObject.GetComponent<PlanetUIManager>().disableUIRing());
            //StartCoroutine(GalaxyCamera.Instance.FollowMothership(trans, inSystem));
            GalaxyCamera.Instance.targetMothership();
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SYSTEM_ZOOM);
        }
    }
    #endregion UnityCallbacks

    #region InternalCallbacks
    
    void OnGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("mothership click");
        destination = worldPosition;
        orbiting = false;
        if (!moving)
        {
            StartCoroutine(Move());
        }
    }

    #endregion InternalCallbacks
    
    #endregion PrivateMethods

    #endregion Methods
}
