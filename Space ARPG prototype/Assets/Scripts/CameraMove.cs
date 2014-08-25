using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour 
{

    private float zoomSpeed = 1000.0f;
    private float minSize = 8.0f;
    private float maxSize = 30.0f;

    ShipMove shipMove;
    Transform _trans;
    Camera _cam;

    void Awake()
    {
        shipMove=GameObject.FindObjectOfType<ShipMove>();
    }
    void Start()
    {
        shipMove.OnShipMoved += shipMove_OnShipMoved;
        InputManager.Instance.OnMouseScroll += OnMouseScroll;
    
        _trans = transform;
        _cam = camera;
    }

    void OnMouseScroll(MouseScrollEventArgs args)
    {
        //_cam.fieldOfView -= args.scrollSpeed * zoomSpeed *Time.deltaTime;
        _cam.orthographicSize -= args.scrollSpeed * zoomSpeed * Time.deltaTime;
        //_cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, minFov, maxFov);
        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, minSize, maxSize);

        FatherTime.timeRate -= args.scrollSpeed;
    }

    void shipMove_OnShipMoved(Transform trans)
    {
        _trans.SetPositionX(trans.position.x);
        _trans.SetPositionZ(trans.position.z);
    }

}
