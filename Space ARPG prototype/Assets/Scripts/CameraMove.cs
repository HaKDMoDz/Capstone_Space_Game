using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour 
{

    public float zoomSpeed = 8f;
    public float minFov = 20.0f;
    public float maxFov = 85.0f;

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
        _cam.fieldOfView -= args.scrollSpeed * zoomSpeed *Time.deltaTime;
        _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView, minFov, maxFov);
    }

    void shipMove_OnShipMoved(Transform trans)
    {
        _trans.SetPositionX(trans.position.x);
        _trans.SetPositionZ(trans.position.z);
    }

}
