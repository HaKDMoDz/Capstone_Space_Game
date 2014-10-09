using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MovementProperties
{
    public float moveForce = 10f;
    public float afterburnerForce = 25f;
    public float maxSpeed = 15f;
    public float decelerationTime = 0.5f;
    public float brakeTime = 0.75f;
}


public class ShipMove : MonoBehaviour
{

    Vector3 velocity = Vector3.zero;

    public MovementProperties moveProps;
    float camHeight;

    //caching vars
    Transform _trans;
    Rigidbody _rbody;

    float lastMousePosX;
    float lastMousePosY;

    float currentVelX;
    float currentVelZ;
    bool afterburnerOn;

    public delegate void ShipMoved(Transform trans);
    public event ShipMoved OnShipMoved = new ShipMoved(delegate (Transform trans){});

    void Start()
    {
        SystemLog.addMessage("Ship Initialized...");
        InputManager.Instance.OnMovementAxis += OnMovementAxis;
        InputManager.Instance.OnMouseMove += OnMouseMove;
        InputManager.Instance.OnKeyboardPress += KeyPress;
        InputManager.Instance.OnMouseClick += MouseClick;

        _trans = transform;
        _rbody = rigidbody;

        camHeight = Camera.main.transform.position.y - _trans.position.y;
        lastMousePosX = Input.mousePosition.x;
        lastMousePosY = Input.mousePosition.y;
    }

    //Input events
    void OnMouseMove(MouseMoveEventArgs args)
    {
        ShipLookAtMouse(args.x, args.y);
        lastMousePosX = args.x;
        lastMousePosY = args.y;

        //ship movement event
        OnShipMoved(_trans);
    }
    void OnMovementAxis(MovementAxisArgs args)
    {

        if(args.horizontal==0.0f && args.vertical==0.0f)
        {
            Brake(moveProps.decelerationTime);
        }

        //constraining speed
        Vector2 horizontalVel = new Vector2(_rbody.velocity.x, _rbody.velocity.z);
        //ship is above max speed
        if(horizontalVel.sqrMagnitude>moveProps.maxSpeed*moveProps.maxSpeed)
        {
            //setting horizontal vel to max speed
            horizontalVel.Normalize();
            horizontalVel *= moveProps.maxSpeed;

            _rbody.SetVelocityX(horizontalVel.x);
            _rbody.SetVelocityZ(horizontalVel.y);
        }

        //moving the ship
        if (afterburnerOn)
        {
            _rbody.AddRelativeForce(args.horizontal * moveProps.afterburnerForce, 0f, args.vertical * moveProps.afterburnerForce);
        }
        else
        {
            _rbody.AddRelativeForce(args.horizontal * moveProps.moveForce, 0f, args.vertical * moveProps.moveForce);
        }

        //making sure the ship looks at the mouse when the ship moves, even if the mouse does not
        ShipLookAtMouse(lastMousePosX, lastMousePosY);

        //ship movement event
        OnShipMoved(_trans);
    }
    void MouseClick(MouseEventArgs args)
    {
        switch (args.button)
        {
            case 0:
                //Debug.Log("Left Mouse Button?");
                break;
            case 1:
                //Debug.Log("Right Mouse Button?");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, 1 << 8))
                {
                    Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, moveProps.maxSpeed * Time.deltaTime * 20f);
                }
                break;
            case 2:
                //Debug.Log("Middle Mouse Button?");
                break;
            default:
                break;
        }
    }
    void KeyPress(KeyboardEventArgs args)
    {
        switch(args.key)
        {
            case KeyCode.Space:
                {
                    Brake(moveProps.brakeTime);
                    break;
                }
            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                {
                    if (args.keyState == KeyboardEventArgs.KeyState.Hold)
                    {
                        afterburnerOn = true;
                    }
                    else if (args.keyState == KeyboardEventArgs.KeyState.Up)
                    {
                        afterburnerOn = false;
                    }
                    break;
                }
        }

    }
    void ShipLookAtMouse(float mouseX, float mouseY)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseX, mouseY, camHeight));
       // transform.LookAt(worldPos);

        //making sure ship doesn't pitch or roll
       // _trans.SetEulerX(0.0f);
       // _trans.SetEulerZ(0.0f);
    }
    void Brake(float brakeTime)
    {
        //slows the speed to zero within the "brakeTime" provided
        _rbody.SetVelocityX(Mathf.SmoothDamp(_rbody.velocity.x, 0.0f, ref currentVelX, brakeTime));
        _rbody.SetVelocityZ(Mathf.SmoothDamp(_rbody.velocity.z, 0.0f, ref currentVelZ, brakeTime));
    }
}
