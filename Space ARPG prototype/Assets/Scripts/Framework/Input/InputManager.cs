using UnityEngine;
using System.Collections;

public partial class InputManager : SingletonComponent<InputManager> {

    //delegates for input related actions

    //for keyboard events
    public delegate void KeyboardAction(KeyboardEventArgs args);
    public delegate void MovementAxisAction(MovementAxisArgs args);

    //mouse events
    public delegate void MouseAction(MouseEventArgs args);
    public delegate void MouseMoveAction(MouseMoveEventArgs args);
    public delegate void MouseScroll(MouseScrollEventArgs args);

    //events raised based on input - register methods to these events
    public event KeyboardAction OnKeyboardPress = new KeyboardAction(delegate(KeyboardEventArgs args) { });//this prevents a null exception, and avoids an if null check before every event raise
    public event MovementAxisAction OnMovementAxis = new MovementAxisAction(delegate(MovementAxisArgs args) { });
    
    public event MouseAction OnMouseClick = new MouseAction(delegate(MouseEventArgs args) { }); 
    public event MouseMoveAction OnMouseMove = new MouseMoveAction(delegate(MouseMoveEventArgs args) { });
    public event MouseScroll OnMouseScroll = new MouseScroll(delegate(MouseScrollEventArgs args) { });

    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        //{
        //    OnKeyboardPress(new KeyboardEventArgs(KeyCode.LeftShift, KeyboardEventArgs.KeyState.Up));
        //    Debug.Log("shift up");
        //}
        //methods that check for input and raise the relevant events
        //defined in the other partial classes

        CheckMouseMove();
        CheckMouseClick();
        CheckMouseScroll();
        
        CheckMovementAxes();
        CheckKeyboardPress();

    }

    
}
