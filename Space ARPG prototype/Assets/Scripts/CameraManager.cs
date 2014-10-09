using UnityEngine;
using System.Collections;

public class CameraManager : SingletonComponent<CameraManager> 
{

    public static float cameraTick = 0.125f;

    public static float zoomTarget;
    private static Camera cam;

    private float epsilon = 0.01f;

    private CameraManager() { }

	void Start () 
    {
        cam = Camera.main;
        InputManager.Instance.OnMouseScroll += OnMouseScroll;
	}

    void OnMouseScroll(MouseScrollEventArgs args)
    {
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Mathf.Clamp(cam.orthographicSize + (10 * -args.scrollSpeed), 5, 30), 1.0f);
        FatherTime.timeRate = cam.orthographicSize/10.0f;
    }
}
