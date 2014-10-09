using UnityEngine;
using System.Collections;

public class CameraManager : SingletonComponent<CameraManager> 
{
    public static float FAR_ZOOM = 30.0f;
    public static float MED_FAR_ZOOM = 20.0f;
    public static float MED_ZOOM = 5.0f;
    public static float CLOSE_ZOOM = 2.0f;

    public static float cameraTick = 0.125f;

    public static float zoomTarget;
    private static Camera cam;

    private float epsilon = 0.01f;

    private CameraManager() { }

	void Start () 
    {
        cam = Camera.main;
        zoomTarget = FAR_ZOOM;
        Debug.Log("Camera Manager Started");
        InputManager.Instance.OnMouseScroll += OnMouseScroll;
	}

    void OnMouseScroll(MouseScrollEventArgs args)
    {

        Debug.Log(args.scrollSpeed);

        if (args.scrollSpeed > 0.05)
        {
            moveUpOneZoomLevel();
        }
        else if (args.scrollSpeed < -0.05)
        {
            MoveDownOneZoomLevel();
        }
    }

    void moveUpOneZoomLevel()
    {
        Debug.Log("zoom out");
        if (zoomTarget == MED_FAR_ZOOM)
        {
            StartCoroutine(changeZoomLevel(FAR_ZOOM));
        }
        else if (zoomTarget == MED_ZOOM)
        {
            StartCoroutine(changeZoomLevel(MED_FAR_ZOOM));
        }
        else if (zoomTarget == CLOSE_ZOOM) 
        {
            StartCoroutine(changeZoomLevel(MED_ZOOM));
        }
    }

    void MoveDownOneZoomLevel()
    {
        if (zoomTarget == MED_ZOOM)
        {
            StartCoroutine(changeZoomLevel(CLOSE_ZOOM));
        }
        else if (zoomTarget == MED_FAR_ZOOM)
        {
            StartCoroutine(changeZoomLevel(MED_ZOOM));
        }
        else if (zoomTarget == FAR_ZOOM)
        {
            StartCoroutine(changeZoomLevel(MED_FAR_ZOOM));
        }
        
        
    }

    public IEnumerator changeZoomLevel(float _target)
    {
        Debug.Log("I'm changing zoom to " + _target);
        zoomTarget = _target;
        while (Mathf.Abs(cam.orthographicSize - _target) > epsilon)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, _target, 0.0005f);
            yield return null;
        }

        yield return null;
    }

    
}
