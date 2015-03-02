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

    public void AddOnClickListener(UnityAction action)
    {
        okButton.onClick.AddListener(action);
    }
}
