using UnityEngine;
using System.Collections;
using System;

public class MovementAxisArgs : EventArgs {

    public float horizontal; 
    public float vertical;

    public MovementAxisArgs(float _hori, float _vert)
    {
        horizontal = _hori;
        vertical = _vert;
    }
}
