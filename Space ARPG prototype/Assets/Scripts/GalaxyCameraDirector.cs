using UnityEngine;
using System.Collections;

public class GalaxyCameraDirector : MonoBehaviour 
{
    public static float FAR_ZOOM = 41.0f;
    public static float MED_ZOOM = 26.0f;
    public static float CLOSE_ZOOM = 14.0f;

    public static float targetZoom;

    Camera cam;

	void Start () 
    {
        cam = camera;
        targetZoom = FAR_ZOOM;

        /*
         * enable this to test procedurally triggered zoom
         * 
        StartCoroutine(WaitFor(5f));
         */
	}
	
	void Update () 
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, 0.02f);

        
	}

    public IEnumerator WaitFor(float seconds)
    {
        Debug.Log("tick");
        yield return new WaitForSeconds(seconds);
        
        targetZoom = MED_ZOOM;
        //this controls where the camera's zoom level "should" be

    }
}
