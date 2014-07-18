using UnityEngine;
using System.Collections;
using System;

public class KeyboardEventArgs : EventArgs {

    public KeyCode key;
    public enum KeyState { Down, Up, Hold}
    public KeyState keyState;

    public KeyboardEventArgs(KeyCode _key, KeyState _keyState)
    {
        key = _key;
        keyState = _keyState;
    }
}
