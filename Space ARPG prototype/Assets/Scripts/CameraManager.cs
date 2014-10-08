using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
    public static float FAR_ZOOM = 41.0f;
    public static float MED_ZOOM = 26.0f;
    public static float CLOSE_ZOOM = 14.0f;

    public static float cameraTick = 0.125f;

    public static float zoomTarget;
    private static Camera cam;

    private static CameraManager _instance = null;

    private CameraManager() { }

	void Start () 
    {
        cam = Camera.main;
        zoomTarget = FAR_ZOOM;
	}

    public static CameraManager getInstance()
    {
        if (_instance == null)
        {
            _instance = new CameraManager();
        }

        return _instance;
    }

    public IEnumerator changeZoomLevel(float _target)
    {
        float epsilon = 1f;
        while (Mathf.Abs(cam.orthographicSize - _target) > epsilon)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, _target, 500f);
        }

        if (Mathf.Abs(cam.orthographicSize - _target) <= epsilon)
        {
            yield return 0;
        }
       
        yield return new WaitForSeconds(cameraTick);
        StartCoroutine(changeZoomLevel(_target));
    }

    
}
