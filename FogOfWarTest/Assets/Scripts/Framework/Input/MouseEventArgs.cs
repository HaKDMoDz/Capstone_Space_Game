using UnityEngine;
using System.Collections;
using System;

public class MouseEventArgs : EventArgs {

    public float x;
    public float y;
    public int button;
    public Vector3 clickWorldPos;
    public GameObject clickedObject;
    public bool buttonUp = false;
    public enum ButtonState {Down, Up, Hold }
    public ButtonState buttonState;

    public MouseEventArgs(float _x, float _y, int _button, ButtonState _buttonState)
    {
        x = _x;
        y = _y;
        button = _button;
        buttonState = _buttonState;
    }
    
}
