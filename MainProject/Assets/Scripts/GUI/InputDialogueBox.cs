using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class InputDialogueBox : MonoBehaviour 
{
    [SerializeField]
    private InputFieldExtended inputFieldEx;
    public InputFieldExtended InputFieldEx
    {
        get { return inputFieldEx; }
        set { inputFieldEx = value; }
    }
    //[SerializeField]
    //private Text inputText;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button cancelButton;

    public InputField GetInputField()
    {
        return inputFieldEx.InputField;
    }
    public void Setup(InputField.CharacterValidation validation, UnityAction<string> onSubmitAction, UnityAction onCancelAction)
    {
        inputFieldEx.InputField.characterValidation = validation;
        AddOnSubmitListener(onSubmitAction);
        AddOnCancelListener(onCancelAction);
    }
    public void AddOnSubmitListener(UnityAction<string> action)
    {
        inputFieldEx.AddOnSubmitListener(action);
        submitButton.onClick.AddListener(() =>action(inputFieldEx.InputField.text));
    }
    public void AddOnCancelListener(UnityAction action)
    {
        inputFieldEx.InputField.onEndEdit.AddListener((value) => ValidateCancelAction(action));
        cancelButton.onClick.AddListener(action);
    }

    private void ValidateCancelAction(UnityAction action)
    {
        if (Input.GetButtonDown("Cancel"))
        {
            action();
        }
    }

    
    
}
