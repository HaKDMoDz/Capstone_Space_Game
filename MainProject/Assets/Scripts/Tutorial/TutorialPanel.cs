using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TutorialPanel : MonoBehaviour 
{
    [SerializeField]
    private Toggle toggle;
    public Toggle Toggle
    {
        get { return toggle; }
    }
    [SerializeField]
    private Button okButton;
    public Button OkButton
    {
        get { return okButton; }
    }
    [SerializeField]
    private bool autoAdvance;
    public bool AutoAdvance
    {
        get { return autoAdvance; }
    }
    [SerializeField]
    private bool turnOffOnOk;
    public bool TurnOffOnOk
    {
        get { return turnOffOnOk; }
    }

    public bool ToggleIsOn
    {
        get { return toggle.isOn; }
    }

    public void AddOnClickListener(UnityAction action)
    {
        okButton.onClick.AddListener(action);
    }
}
