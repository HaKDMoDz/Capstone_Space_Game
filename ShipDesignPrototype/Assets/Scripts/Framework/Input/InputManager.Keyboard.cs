using UnityEngine;
using System.Collections;

public partial class InputManager
{
    
    //checks Movement axes
    void CheckMovementAxes()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        OnMovementAxis(new MovementAxisArgs(horizontal, vertical));
    }
    
    //checks for keyboard input
    void CheckKeyboardPress()
    {
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha1, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha1))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha1, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha2, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha2, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha3, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha3, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha4, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha4, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha5, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha5, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha6, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha6, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha7, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha7, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha1, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha8))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha8, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha9, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha9))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha9, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha0, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Alpha0))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Alpha0, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.W, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.W))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.W, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.A, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.A))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.A, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.S, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.S))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.S, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.D, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.D))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.D, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.F, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.F))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.F, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Q, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Q))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Q, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.E, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.E))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.E, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.I, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.I))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.I, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Escape, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Escape, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Space, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.Space, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.LeftShift, KeyboardEventArgs.KeyState.Down));
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.LeftShift, KeyboardEventArgs.KeyState.Hold));
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                OnKeyboardPress(new KeyboardEventArgs(KeyCode.LeftShift, KeyboardEventArgs.KeyState.Up));
            }
        }

    }

}
