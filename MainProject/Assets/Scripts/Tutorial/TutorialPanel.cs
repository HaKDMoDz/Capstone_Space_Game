using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class TutorialPanel : MonoBehaviour 
{
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Button okButton;
    [SerializeField]
    private bool autoAdvance;
    public bool AutoAdvance
    {
        get { return autoAdvance; }
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
