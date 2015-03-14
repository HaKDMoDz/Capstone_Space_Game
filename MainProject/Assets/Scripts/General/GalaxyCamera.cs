using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CamTarget {MOTHERSHIP, TARGET_PLANET }
public enum CamZoomLevel { SPACE_ZOOM, SYSTEM_ZOOM, PLANET_ZOOM }

public class GalaxyCamera : Singleton<GalaxyCamera>
{
    #region Fields
    
    //EditorExposed
    private float orbitZoomHeight = 120.0f;
    private float spaceZoomHeight = 200.00f;
    private float systemZoomHeight = 100.0f;

    //Cached references
    private Transform trans;
    private Quaternion initialRot;
    private float initialAngleX;
    private float epsilon = 0.01f;

    public GameObject mothership;
    public GameObject currentTarget;

    private Vector3 targetPosition;
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }
    private Quaternion targetRotation;
    public Quaternion TargetRotation
    {
        get { return targetRotation; }
        set { targetRotation = value; }
    }
    private CamTarget targetType;
    public CamTarget TargetType
    {
        get { return targetType; }
        set { targetType = value; }
    }
    private CamZoomLevel zoomLevel;
    public CamZoomLevel ZoomLevel
    {
        get { return zoomLevel; }
        set { zoomLevel = value; }
    }

    private float currentZoom;

    //Events
    public delegate void CameraMoveEvent();
    public event CameraMoveEvent OnCameraMove = new CameraMoveEvent(() => { });

    #endregion Fields

    #region Methods

    public void changeZoomLevel(CamZoomLevel _newZoom)
    {
        zoomLevel = _newZoom;
    }

    public void targetPlanet(Transform _planet)
    {
        targetPosition = _planet.position;
        targetType = CamTarget.TARGET_PLANET;
    }

    public void targetMothership()
    {
        targetPosition = mothership.transform.position;
        targetType = CamTarget.MOTHERSHIP;
    }

    private void Awake()
    {
        trans = transform;
        initialRot = trans.rotation;
        initialAngleX = Mathf.Deg2Rad * initialRot.eulerAngles.x;
        targetRotation = initialRot;
        zoomLevel = CamZoomLevel.SPACE_ZOOM;
        targetMothership();

    }
    #endregion Methods

    private void Update()
    {

        switch (targetType)
        {
            case CamTarget.MOTHERSHIP:
                currentTarget = mothership;
                break;
            case CamTarget.TARGET_PLANET:
                
            default:
                break;
        }

        if ((trans.rotation.eulerAngles - targetRotation.eulerAngles).magnitude > epsilon)
        {
            //lerp rotation
            trans.rotation = Quaternion.Slerp(trans.rotation, targetRotation, 0.1f);
        }

        float targetZoom;
        switch (zoomLevel)
        {
            case CamZoomLevel.SPACE_ZOOM:
                //Debug.Log("Zoom: Space");
                targetZoom = spaceZoomHeight;
                break;
            case CamZoomLevel.SYSTEM_ZOOM:
                //Debug.Log("Zoom: System");
                targetZoom = systemZoomHeight;
                break;
            case CamZoomLevel.PLANET_ZOOM:
                //Debug.Log("Zoom: Planet");
                targetZoom = orbitZoomHeight;
                break;
            default:
                targetZoom = -1.0f;
                break;
        }

        Vector3 targetPosWithZoom = targetPosition;
        targetPosWithZoom.y += targetZoom;
        targetPosWithZoom.z -= targetZoom / Mathf.Tan(initialAngleX);

        if ((trans.position - targetPosWithZoom).magnitude > epsilon)
        {
            //lerp zoom
            trans.position = Vector3.Lerp(trans.position, targetPosWithZoom, 0.1f);
        }
        //trans.LookAt(targetPosition);
    }
}
