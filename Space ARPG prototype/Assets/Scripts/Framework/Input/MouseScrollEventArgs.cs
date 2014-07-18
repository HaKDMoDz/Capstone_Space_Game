using UnityEngine;
using System.Collections;
using System;

public class MouseScrollEventArgs : EventArgs 
{
    public float scrollSpeed;

    public MouseScrollEventArgs(float _scrollSpeed)
    {
        scrollSpeed = _scrollSpeed;
    }

}
