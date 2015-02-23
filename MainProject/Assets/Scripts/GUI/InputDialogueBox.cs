using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class InputDialogueBox : MonoBehaviour 
{
    [SerializeField]
    private InputFieldExtended inputFieldEx;
    public InputFieldExtended InputFieldEx
    {
        get { return inputFieldEx; }
        set { inputFieldEx = value; }
    }
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button cancelButton;

    Regex inputValidateRegex = new Regex(@"^[\w\-. ]+$");

    public InputField GetInputField()
    {
        return inputFieldEx.InputField;
    }
    public void Setup(UnityAction<string> onSubmitAction, UnityAction onCancelAction)
    {
        AddOnSubmitListener(onSubmitAction);
        AddOnCancelListener(onCancelAction);
        inputFieldEx.InputField.onValidateInput += ValidateInput;
    }
    private char ValidateInput(string str, int num, char chr)
    {
        return inputValidateRegex.IsMatch(chr.ToString()) ? chr : '\0';
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
