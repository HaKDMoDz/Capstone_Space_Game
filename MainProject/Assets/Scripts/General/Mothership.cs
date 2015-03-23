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
    private bool orbiting;

    private float angle = 0.0f;

    public bool Orbiting { get { return orbiting; } set { orbiting = value; } }

    public int orbitID = -1;

    //cached
    private Transform trans;

    private Vector3 destination;
    private Vector3 orbitDestination;
    #endregion Fields

    #region Methods

    #region PrivateMethods

    void Update()
    {
            Vector3 moveDir;
            moveDir = destination - trans.position;

            if (moveDir.magnitude > moveSpeed * Time.deltaTime && moveDir.magnitude > GlobalVars.LerpDistanceEpsilon && !orbiting)
            {
                Vector3 moveDirNorm = moveDir.normalized;
                trans.LookAt(destination);
                trans.position += moveDirNorm * moveSpeed * Time.deltaTime;
                GalaxyCamera.Instance.targetMothership();  
            }
            trans.LookAt(destination);

       
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
        destination = trans.position;
    }

    private void SaveData()
    {
        GameController.Instance.GameData.galaxyMapData.position = trans.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagsAndLayers.SolarSystemTag)
        {
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SYSTEM_ZOOM);
        }
        if (other.tag == TagsAndLayers.PlanetTag)
        {
            Transform otherTrans = other.transform;
            float deltaZ = otherTrans.position.z - trans.position.z;
            float deltaX = otherTrans.position.x - trans.position.x;
            angle = (180.0f + ((Mathf.Atan2(deltaZ,deltaX) * 180.0f) / Mathf.PI))%360.0f;
            orbiting = true;
            orbitID = otherTrans.gameObject.GetComponent<Planet_Dialogue>().ID;
            StartCoroutine(otherTrans.gameObject.GetComponent<PlanetUIManager>().enableUIRing());
            GalaxyCamera.Instance.targetPlanet(otherTrans);
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.PLANET_ZOOM);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == TagsAndLayers.PlanetTag && orbiting)
        {
            angle = (angle + orbitSpeed) % 360.0f;
            Transform otherTrans = other.transform;
            trans.position = PointOnCircle(((SphereCollider)other).radius - 10.0f, angle, otherTrans.position);
            destination = PointOnCircle(((SphereCollider)other).radius - 10.0f, (angle + 2.0f) % 360.0f, otherTrans.position);
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
            GalaxyCamera.Instance.targetMothership();
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SPACE_ZOOM);
        }
        if (other.tag == TagsAndLayers.PlanetTag && orbitID == other.gameObject.GetComponent<Planet_Dialogue>().ID)
        {
            orbiting = false;
            StartCoroutine(other.gameObject.GetComponent<PlanetUIManager>().disableUIRing());
            GalaxyCamera.Instance.targetMothership();
            GalaxyCamera.Instance.changeZoomLevel(CamZoomLevel.SYSTEM_ZOOM);
        }
    }
    #endregion UnityCallbacks

    #region InternalCallbacks
    
    void OnGroundClick(Vector3 worldPosition)
    {
        orbiting = false;
        destination = worldPosition;
    }

    #endregion InternalCallbacks
    
    #endregion PrivateMethods

    #endregion Methods
}
