using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public int groundLayer;
    public float movementEpsilon = 0.2f;

    Vector3 destination;
    bool moving = false;
    Transform _trans;

    void Start()
    {
        _trans = transform;
        InputManager.Instance.OnMouseClick += MouseClick;
    }

    public IEnumerator StartTurn()
    {
        Debug.Log("Start turn");
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (moving)
            {
                Vector3 moveDir = destination - _trans.position;
                if (Vector3.SqrMagnitude(moveDir) > movementEpsilon * movementEpsilon)
                {
                    _trans.Translate(moveDir * Time.deltaTime);

                }
                else
                {
                    moving = false;
                    Debug.Log("end move");
                }
            }
            yield return null;
        }
        moving = false;
    }

    void MouseClick(MouseEventArgs args)
    {
        if (args.button == 1 && args.buttonState == MouseEventArgs.ButtonState.Down)
        {
            MoveTo(args.x, args.y);
        }
    }
    void MoveTo(float x, float y)
    {
        Debug.Log("move");

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, 1 << groundLayer))
        {
            destination = hit.point;
            destination.y = _trans.position.y;
            moving = true;
        }
    }


}
