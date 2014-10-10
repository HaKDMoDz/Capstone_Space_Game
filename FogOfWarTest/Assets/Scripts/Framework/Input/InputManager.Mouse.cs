using UnityEngine;
using System.Collections;

public partial class InputManager
{


    /// <summary>
    /// Raises the mouse click event along with mouseEventArgs that contains the info regarding the mouse click
    /// </summary>
    void CheckMouseClick()
    {
        if (Input.anyKey)
        {


            //left clicks
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 0, MouseEventArgs.ButtonState.Down));
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 0, MouseEventArgs.ButtonState.Up));
            }
            if (Input.GetMouseButton(0))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 0, MouseEventArgs.ButtonState.Hold));
            }

            //right clicks
            if (Input.GetMouseButtonDown(1))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 1, MouseEventArgs.ButtonState.Down));
            }
            if (Input.GetMouseButtonUp(1))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 1, MouseEventArgs.ButtonState.Up));
            }
            if (Input.GetMouseButton(1))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 1, MouseEventArgs.ButtonState.Hold));
            }

            //middle clicks
            if (Input.GetMouseButtonDown(2))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 1, MouseEventArgs.ButtonState.Down));
            }
            if (Input.GetMouseButtonUp(2))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 1, MouseEventArgs.ButtonState.Up));
            }
            if (Input.GetMouseButton(2))
            {
                OnMouseClick(new MouseEventArgs(Input.mousePosition.x, Input.mousePosition.y, 2, MouseEventArgs.ButtonState.Hold));
            }
        }
    }

    //raises the mouse moved event with the mouse move args
    void CheckMouseMove()
    {
        float mouseXAxis = Input.GetAxis("Mouse X");
        float mouseYAxis = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mouseXAxis) > 0.0f || Mathf.Abs(mouseYAxis) > 0.0f)
        {
            Vector2 moveDir = new Vector2(mouseXAxis, mouseYAxis);
            OnMouseMove(new MouseMoveEventArgs(Input.mousePosition.x, Input.mousePosition.y, moveDir, false, 0));
        }
    }

    //raises mouse scroll event 
    void CheckMouseScroll()
    {
        float scrollSpeed = Input.GetAxis ("Mouse ScrollWheel");
        if(scrollSpeed!=0.0f)
        {
            OnMouseScroll(new MouseScrollEventArgs(scrollSpeed));
        }
    }


}
