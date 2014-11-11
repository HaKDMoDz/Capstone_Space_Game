using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum MouseButton { Left = 0, Right = 1, Middle = 2 }

public partial class InputManager : Singleton<InputManager>
{
    //mouse events
    public delegate void MouseClickEvent(MouseButton mouseButton);
    public delegate void MouseMoveEvent(Vector2 direction);
    /// <summary>
    /// This event is raised whenever the mouse is moved - the argument is a vector of the horizontal and vertical movement of the mouse
    /// </summary>
    public event MouseMoveEvent OnMouseMoveEvent = new MouseMoveEvent((Vector2) => { });
    public delegate void MouseScrollEvent(float scrollSpeed);
    /// <summary>
    /// This event is raised whenever the mouse scroll wheel is spun - the argument is the speed in which it was spun (-ve means down)
    /// </summary>
    public event MouseScrollEvent OnMouseScrollEvent = new MouseScrollEvent((float scrollSpeed) => { });


    private List<MouseButton> buttonsToCheck;
    private Dictionary<MouseButton, MouseClickEvent> mouseDownEvents, mouseUpEvents, mouseHoldEvents;

    //keyboard events
    public delegate void KeyPressEvent(KeyCode key);
    public delegate void KeyboardAxisEvent(Vector2 direction);
    /// <summary>
    /// This event is raised whenever the keyboard axis is activated - the argument is a vector of the horizontal and vertical axes
    /// </summary>
    public event KeyboardAxisEvent OnKeyboardAxisEvent = new KeyboardAxisEvent((Vector2) => { });


    private List<KeyCode> keysToCheck;
    private Dictionary<KeyCode, KeyPressEvent> keyDownEvents, keyUpEvents, keyHoldEvents;

    private bool initialized = false;

    void Awake()
    {
        Init();
    }
    void Init()
    {
        keysToCheck = new List<KeyCode>();
        keyDownEvents = new Dictionary<KeyCode, KeyPressEvent>();
        keyUpEvents = new Dictionary<KeyCode, KeyPressEvent>();
        keyHoldEvents = new Dictionary<KeyCode, KeyPressEvent>();
        buttonsToCheck = new List<MouseButton>();
        mouseDownEvents = new Dictionary<MouseButton, MouseClickEvent>();
        mouseUpEvents = new Dictionary<MouseButton, MouseClickEvent>();
        mouseHoldEvents = new Dictionary<MouseButton, MouseClickEvent>();

        initialized = true;
    }
    void Update()
    {
        CheckMouseMove();
        CheckMouseScroll();
        CheckMouseClick();
        
        CheckKeyboardAxes();
        CheckKeyboardPress();
    }
}
