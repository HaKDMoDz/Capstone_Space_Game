using UnityEngine;
using System.Collections;

public partial class InputManager : SingletonComponent<InputManager> {

    //delegates for input related actions

    //for key presses and clicks
    public delegate void MouseAction(MouseEventArgs args);
    public delegate void KeyboardAction(KeyboardEventArgs args);

    //based on mouse axis and movement axis
    public delegate void MouseMoveAction(MouseMoveEventArgs args);
    public delegate void MovementAxisAction(MovementAxisArgs args);

    //events raised based on input - register methods to these events
    public event MouseAction OnMouseClick = new MouseAction(delegate (MouseEventArgs args){}); //this prevents a null exception, and avoids an if null check before every event raise
    public event KeyboardAction OnKeyboardPress=new KeyboardAction(delegate (KeyboardEventArgs args){});
    
    public event MouseMoveAction OnMouseMove=new MouseMoveAction(delegate (MouseMoveEventArgs args){});
    public event MovementAxisAction OnMovementAxis = new MovementAxisAction(delegate (MovementAxisArgs args){});

    void Update()
    {
        //methods that check for input and raise the relevant events
        //defined in the other partial classes
        CheckMouseMove();
        CheckMovementAxes();
        
        CheckMouseClick();
        CheckKeyboardPress();
    }

    
}
