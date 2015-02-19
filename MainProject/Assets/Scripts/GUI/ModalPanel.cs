#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion Usings

public class ModalPanel : MonoBehaviour
{
    #region Fields
    //Editor Exposed
    [SerializeField]
    private ButtonWithContent yesButton;
    [SerializeField]
    private ButtonWithContent noButton;
    [SerializeField]
    private ButtonWithContent cancelButton;
    [SerializeField]
    private Text message;
    #endregion Fields

    /// <summary>
    /// Show a modal box with a message and an ok button. It will call the okAction once the OK button is clicked and will de-activate itself.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="okAction"></param>
    public void ShowMessageWithOk(string message, UnityAction okAction)
    {
        SetMessage(message);
        ShowButtons(true, false, false);
        yesButton.buttonText.text = "Ok";
        SetActionForButton(yesButton,
            () => 
            { 
                okAction();
                yesButton.buttonText.text = "Yes";
            });
    }
    /// <summary>
    /// Show a modal box with a message; yes button and cancel button. The corresponding actions will be called by the buttons.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="yesAction"></param>
    /// <param name="cancelAction"></param>
    public void ShowMessageWithYesCancel(string message, UnityAction yesAction, UnityAction cancelAction)
    {
        SetMessage(message);
        ShowButtons(true, false, true);
        SetActionForButton(yesButton, yesAction);
        SetActionForButton(cancelButton, cancelAction);
    }

    private void SetMessage(string message)
    {
        #if FULL_DEBUG || LOW_DEBUG
        if(String.IsNullOrEmpty(message))
        {
            Debug.LogError("Empty message");
            this.message.text = "";
        }
        #endif
        this.message.text = message;
    }
    private void SetActionForButton(ButtonWithContent button, UnityAction action)
    {
        button.button.onClick.RemoveAllListeners();
        button.button.onClick.AddListener(
            () =>
            {
                if(action != null) action();
                gameObject.SetActive(false);
            });
    }
    private void ShowButtons(bool showYes, bool showNo, bool showCancel)
    {
        yesButton.gameObject.SetActive(showYes);
        noButton.gameObject.SetActive(showNo);
        cancelButton.gameObject.SetActive(showCancel);
    }
}
