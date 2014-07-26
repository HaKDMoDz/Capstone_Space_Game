using UnityEngine;
using System.Collections;
using System;

public class MouseMoveEventArgs : EventArgs {

    public float x; //current x, y positions
    public float y;
    public Vector2 mouseMoveDir; 
    public bool drag; //if it is being dragged or not
    public int button; //if dragged, then the button being held

    public MouseMoveEventArgs(float _x, float _y, Vector2 _dir, bool _drag, int _button)
    {
        x = _x;
        y = _y;
        mouseMoveDir = _dir;
        drag = _drag;
        button = _button;
    }

}
