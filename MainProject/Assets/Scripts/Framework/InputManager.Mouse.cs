/*
  InputManager.Mouse.cs
  Mission: Invasion
  Created by Rohun Banerji on Nov 10/2014
  Copyright (c) 2014 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class InputManager
{
    #region Methods

    #region Public
    /// <summary>
    /// Register a method to hook up to MouseDown events and the Mouse Buttons to listen for
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    /// Method to call when the Mouse down event occurs
    /// <param name="buttons">
    /// The Mouse Buttons to listen for (pass in multiple buttons separated by commas)
    /// </param>
    public void RegisterMouseButtonsDown(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            RegisterMouseButtonDown(mouseClickEvent, button);
        }
    }
    /// <summary>
    /// Register a method to hook up to Mouse Up events and the Mouse Buttons to listen for
    /// </summary>
    /// <param name="mouseClickEvent">
    /// Method to call when the Mouse up event occurs
    /// </param>
    /// <param name="buttons">
    /// The Mouse Buttons to listen for (pass in multiple buttons separated by commas)
    /// </param>
    public void RegisterMouseButtonsUp(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            RegisterMouseButtonUp(mouseClickEvent, button);
        }
    }
    /// <summary>
    /// Register a method to hook up to Mouse Hold events and the Mouse Buttons to listen for
    /// </summary>
    /// <param name="mouseClickEvent">
    /// Method to call when the Mouse Hold event occurs
    /// </param>
    /// <param name="buttons">
    /// The Mouse Buttons to listen for (pass in multiple buttons separated by commas)
    /// </param>
    public void RegisterMouseButtonsHold(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            RegisterMouseButtonHold(mouseClickEvent, button);
        }
    }
    /// <summary>
    /// De-register methods from the event for the passed Mouse Buttons
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    /// <param name="buttons"></param>
    public void DeRegisterMouseButtonsDown(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            DeRegisterMouseButtonDown(mouseClickEvent, button);
        }
    }
    /// <summary>
    /// De-register methods from the event for the passed Mouse Buttons
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    /// <param name="buttons"></param>
    public void DeRegisterMouseButtonsUp(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            DeRegisterMouseButtonUp(mouseClickEvent, button);
        }
    }
    /// <summary>
    /// De-register methods from the event for the passed Mouse Buttons
    /// </summary>
    /// <param name="mouseClickEvent"></param>
    /// <param name="buttons"></param>
    public void DeRegisterMouseButtonsHold(MouseClickEvent mouseClickEvent, params MouseButton[] buttons)
    {
        if (!initialized)
        {
            Init();
        }
        foreach (MouseButton button in buttons)
        {
            DeRegisterMouseButtonHold(mouseClickEvent, button);
        }
    }
    #endregion //Public Methods

    #region Private
    private void CheckMouseClick()
    {
        foreach (MouseButton button in buttonsToCheck)
        {
            if (Input.GetMouseButtonDown((int)button))
            {
                MouseDown(button);
            }
            if (Input.GetMouseButtonUp((int)button))
            {
                MouseUp(button);
            }
            if (Input.GetMouseButton((int)button))
            {
                MouseHold(button);
            }
        }
    }
    private void CheckMouseMove()
    {
        float mouseXAxis = Input.GetAxis("Mouse X");
        float mouseYAxis = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mouseXAxis) > 0.0f || Mathf.Abs(mouseYAxis) > 0.0f)
        {
            OnMouseMoveEvent(new Vector2(mouseXAxis, mouseYAxis));
        }
    }
    private void CheckMouseScroll()
    {
        float scrollSpeed = Input.GetAxis("Mouse ScrollWheel");
        if (scrollSpeed != 0.0f)
        {
            OnMouseScrollEvent(scrollSpeed);
        }
    }
    private void RegisterMouseButtonDown(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseDownEvents.ContainsKey(button))
        {
            mouseDownEvents[button] += mouseClickEvent;
        }
        else
        {
            AddButtonToCheck(button);
            mouseDownEvents.Add(button, mouseClickEvent);
        }
    }
    private void RegisterMouseButtonUp(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseUpEvents.ContainsKey(button))
        {
            mouseUpEvents[button] += mouseClickEvent;
        }
        else
        {
            AddButtonToCheck(button);
            mouseUpEvents.Add(button, mouseClickEvent);
        }
    }
    private void RegisterMouseButtonHold(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseHoldEvents.ContainsKey(button))
        {
            mouseHoldEvents[button] += mouseClickEvent;
        }
        else
        {
            AddButtonToCheck(button);
            mouseHoldEvents.Add(button, mouseClickEvent);
        }
    }
    private void DeRegisterMouseButtonDown(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseDownEvents.ContainsKey(button))
        {
            mouseDownEvents[button] -= mouseClickEvent;
            if (mouseDownEvents[button] == null)
            {
                mouseDownEvents.Remove(button);
            }
            CheckAndRemoveButton(button);
        }
        else
        {
            Debug.Log(button + "button is not registered");
        }
    }
    private void DeRegisterMouseButtonUp(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseUpEvents.ContainsKey(button))
        {
            mouseUpEvents[button] -= mouseClickEvent;
            if (mouseUpEvents[button] == null)
            {
                mouseUpEvents.Remove(button);
            }
            CheckAndRemoveButton(button);
        }
        else
        {
            Debug.Log(button + "button is not registered");
        }
    }
    private void DeRegisterMouseButtonHold(MouseClickEvent mouseClickEvent, MouseButton button)
    {
        if (mouseHoldEvents.ContainsKey(button))
        {
            mouseHoldEvents[button] -= mouseClickEvent;
            if (mouseHoldEvents[button] == null)
            {
                mouseHoldEvents.Remove(button);
            }
            CheckAndRemoveButton(button);
        }
        else
        {
            Debug.Log(button + "button is not registered");
        }
    }
    private void AddButtonToCheck(MouseButton button)
    {
        if (!buttonsToCheck.Contains(button))
        {
            buttonsToCheck.Add(button);
        }
    }
    private void CheckAndRemoveButton(MouseButton button)
    {
        if (!mouseDownEvents.ContainsKey(button)
            && !mouseUpEvents.ContainsKey(button)
            && !mouseHoldEvents.ContainsKey(button))
        {
            buttonsToCheck.Remove(button);
        }
    }
    private void MouseDown(MouseButton button)
    {
        MouseClickEvent mouseClickEvent = null;
        if (mouseDownEvents.TryGetValue(button, out mouseClickEvent))
        {
            if (mouseClickEvent != null)
            {
                mouseClickEvent(button);
            }
        }
    }
    private void MouseUp(MouseButton button)
    {
        MouseClickEvent mouseClickEvent = null;
        if (mouseUpEvents.TryGetValue(button, out mouseClickEvent))
        {
            if (mouseClickEvent != null)
            {
                mouseClickEvent(button);
            }
        }
    }
    private void MouseHold(MouseButton button)
    {
        MouseClickEvent mouseClickEvent = null;
        if (mouseHoldEvents.TryGetValue(button, out mouseClickEvent))
        {
            if (mouseClickEvent != null)
            {
                mouseClickEvent(button);
            }
        }
    }
    #endregion //Private Methods
    #endregion//Methods

}
