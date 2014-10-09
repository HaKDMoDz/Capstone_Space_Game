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

    private CameraManager() { }

	void Start () 
    {
        cam = Camera.main;
        zoomTarget = FAR_ZOOM;
	}

    public IEnumerator changeZoomLevel(float _target)
    {
        float epsilon = 1f;
        while (Mathf.Abs(cam.orthographicSize - _target) > epsilon)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, _target, 0.05f);
            yield return null;
        }

        yield return null;
    }

    
}
