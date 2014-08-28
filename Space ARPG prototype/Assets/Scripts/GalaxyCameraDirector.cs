using UnityEngine;
using System.Collections;

public class GalaxyCameraDirector : MonoBehaviour 
{
    public static float FAR_ZOOM = 25.0f;
    public static float MED_ZOOM = 15.0f;
    public static float CLOSE_ZOOM = 8.0f;

    private float targetZoom;

    Camera cam;

	void Start () 
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;

        StartCoroutine(WaitFor(5f));
	}
	
	void Update () 
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, 0.1f);
	}

    public IEnumerator WaitFor(float seconds)
    {
        Debug.Log("tick");
        yield return new WaitForSeconds(seconds);
        targetZoom = MED_ZOOM;

    }
}
