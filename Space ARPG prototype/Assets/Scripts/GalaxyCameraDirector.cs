using UnityEngine;

public class GalaxyCameraDirector : MonoBehaviour 
{
    //public static float targetZoom;

    Camera cam;

	void Start () 
    {
        cam = camera;
	}
	
	void Update () 
    {
       // cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, 0.02f);
	}
}
