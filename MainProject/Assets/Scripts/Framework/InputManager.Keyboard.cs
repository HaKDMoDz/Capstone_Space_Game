using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class InputManager
{
    #region Methods

    #region Public
    /// <summary>
    /// Register a method to hook up to KeyDown events and the KeyCodes to listen for
    /// </summary>
    /// <param name="keyEvent">
    /// Method to call when the Keydown event occurs
    /// </param>
    /// <param name="keys">
    /// The Keycodes to listen for (pass in multiple keys separated by commas)
    /// </param>
    public void RegisterKeysDown(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            RegisterKeyDown(keyEvent, key);
        }
    }
    /// <summary>
    /// Register a method to hook up to KeyUp events and the KeyCodes to listen for
    /// </summary>
    /// <param name="keyEvent">
    /// Method to call when the KeyUp event occurs
    /// </param>
    /// <param name="keys">
    /// The Keycodes to listen for (pass in multiple keys separated by commas)
    /// </param>
    public void RegisterKeysUp(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            RegisterKeyUp(keyEvent, key);
        }
    }
    /// <summary>
    /// Register a method to hook up to KeyHold events and the KeyCodes to listen for
    /// </summary>
    /// <param name="keyEvent">
    /// Method to call when the KeyHold event occurs
    /// </param>
    /// <param name="keys">
    /// The Keycodes to listen for (pass in multiple keys separated by commas)
    /// </param>
    public void RegisterKeysHold(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            RegisterKeyHold(keyEvent, key);
        }
    }
    /// <summary>
    /// Deregister methods from the event for the passed KeyCodes
    /// </summary>
    /// <param name="keyEvent"></param>
    /// <param name="keys"></param>
    public void DeregisterKeysDown(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            DeregisterKeyDown(key, keyEvent);
        }
    }
    /// <summary>
    /// Deregister methods from the event for the passed KeyCodes
    /// </summary>
    /// <param name="keyEvent"></param>
    /// <param name="keys"></param>
    public void DeregisterKeysUp(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            DeregisterKeyUp(key, keyEvent);
        }
    }
    /// <summary>
    /// Deregister methods from the event for the passed KeyCodes
    /// </summary>
    /// <param name="keyEvent"></param>
    /// <param name="keys"></param>
    public void DeregisterKeysHold(KeyPressEvent keyEvent, params KeyCode[] keys)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (KeyCode key in keys)
        {
            DeregisterKeyHold(key, keyEvent);
        }
    }
    #endregion //Public Methods

    #region Private
    private void CheckKeyboardPress()
    {
        foreach (KeyCode key in keysToCheck)
        {
            if (Input.GetKeyDown(key))
            {
                KeyDown(key);
            }
            if (Input.GetKeyUp(key))
            {
                KeyUp(key);
            }
            if (Input.GetKey(key))
            {
                KeyHold(key);
            }
        }
    }
    private void CheckKeyboardAxes()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0.0f || Mathf.Abs(vertical) > 0.0f)
        {
            OnKeyboardAxisEvent(new Vector2(horizontal, vertical));
        }
    }
    private void RegisterKeyDown(KeyPressEvent keyEvent, KeyCode key)
    {
        if (keyDownEvents.ContainsKey(key))
        {
            keyDownEvents[key] += keyEvent;
        }
        else
        {
            AddKeyToCheck(key);
            keyDownEvents.Add(key, keyEvent);
        }
    }

    private void RegisterKeyUp(KeyPressEvent keyEvent, KeyCode key)
    {
        if (keyUpEvents.ContainsKey(key))
        {
            keyUpEvents[key] += keyEvent;
        }
        else
        {
            AddKeyToCheck(key);
            keyUpEvents.Add(key, keyEvent);
        }
    }

    private void RegisterKeyHold(KeyPressEvent keyEvent, KeyCode key)
    {
        if (keyHoldEvents.ContainsKey(key))
        {
            keyHoldEvents[key] += keyEvent;
        }
        else
        {
            AddKeyToCheck(key);
            keyHoldEvents.Add(key, keyEvent);
        }
    }

    private void DeregisterKeyDown(KeyCode key, KeyPressEvent keyEvent)
    {
        //print("Deregister key down" + key);
        if (keyDownEvents.ContainsKey(key))
        {
            keyDownEvents[key] -= keyEvent;
            if (keyDownEvents[key] == null)
            {
                keyDownEvents.Remove(key);
            }
            CheckAndRemoveKey(key);
        }
        else
        {
            Debug.Log(key + " Key is not registered");
        }
    }

    private void DeregisterKeyUp(KeyCode key, KeyPressEvent keyEvent)
    {
        print("Deregister key up" + key);
        if (keyUpEvents.ContainsKey(key))
        {
            keyUpEvents[key] -= keyEvent;
            if (keyUpEvents[key] == null)
            {
                keyUpEvents.Remove(key);
            }
            CheckAndRemoveKey(key);
        }
        else
        {
            Debug.Log(key + " Key is not registered");
        }
    }

    private void DeregisterKeyHold(KeyCode key, KeyPressEvent keyEvent)
    {
        print("Deregister key hold" + key);
        if (keyHoldEvents.ContainsKey(key))
        {
            keyHoldEvents[key] -= keyEvent;
            if (keyHoldEvents[key] == null)
            {
                keyHoldEvents.Remove(key);
            }
            CheckAndRemoveKey(key);
        }
        else
        {
            Debug.Log(key + " Key is not registered");
        }
    }


    private void AddKeyToCheck(KeyCode key)
    {
        if (!keysToCheck.Contains(key))
        {
            keysToCheck.Add(key);
        }
    }
    private void CheckAndRemoveKey(KeyCode key)
    {
        if (!keyUpEvents.ContainsKey(key) && !keyDownEvents.ContainsKey(key) && !keyHoldEvents.ContainsKey(key))
        {
            keysToCheck.Remove(key);
        }
    }

    private void KeyDown(KeyCode key)
    {
        KeyPressEvent keyEvent = null;
        if (keyDownEvents.TryGetValue(key, out keyEvent))
        {
            if (keyEvent != null)
            {
                keyEvent(key);
            }
        }
    }
    private void KeyUp(KeyCode key)
    {
        KeyPressEvent keyEvent = null;
        if (keyUpEvents.TryGetValue(key, out keyEvent))
        {
            if (keyEvent != null)
            {
                keyEvent(key);
            }
        }
    }
    private void KeyHold(KeyCode key)
    {
        KeyPressEvent keyEvent = null;
        if (keyHoldEvents.TryGetValue(key, out keyEvent))
        {
            if (keyEvent != null)
            {
                keyEvent(key);
            }
        }
    }
    #endregion //Private Methods
    #endregion //Methods
}
