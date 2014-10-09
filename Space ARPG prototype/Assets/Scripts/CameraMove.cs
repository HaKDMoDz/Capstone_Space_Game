using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour 
{

    ShipMove shipMove;
    Transform _trans;

    void Awake()
    {
        shipMove=GameObject.FindObjectOfType<ShipMove>();
    }
    void Start()
    {
        shipMove.OnShipMoved += shipMove_OnShipMoved;
        InputManager.Instance.OnMouseScroll += OnMouseScroll;
    
        _trans = transform;
    }
    
    void OnMouseScroll(MouseScrollEventArgs args)
    {
        FatherTime.timeRate -= args.scrollSpeed;
    }
    
    void shipMove_OnShipMoved(Transform trans)
    {
        _trans.SetPositionX(trans.position.x);
        _trans.SetPositionZ(trans.position.z);
    }

}
