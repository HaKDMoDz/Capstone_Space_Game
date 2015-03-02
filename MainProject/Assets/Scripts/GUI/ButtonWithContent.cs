using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonWithContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button;
    public Button Button
    {
        get { return button; }
    }
    [SerializeField]
    private Text buttonText;

    private delegate void ButtonPointerEnterEvent();
    private event ButtonPointerEnterEvent OnButtonPointerEnter = new ButtonPointerEnterEvent(() => { });
    private delegate void ButtonPointerExitEvent();
    private event ButtonPointerExitEvent OnButtonPointerExit = new ButtonPointerExitEvent(() => { });

    public void SetText(string text)
    {
        #if FULL_DEBUG
        if(String.IsNullOrEmpty(text))
        {
            Debug.LogError("Button text is null or empty");
            buttonText.text = "";
            return;
        }
        #endif
        buttonText.text = text;
    }

    public void AddOnClickListener(UnityAction action)
    {
        button.onClick.AddListener(action);
    }
    public void RemoveOnClickListeners()
    {
        button.onClick.RemoveAllListeners();
    }
    public void AddOnPointerEnterListener(UnityAction action)
    {
        OnButtonPointerEnter += ()=>action();
    }
    public void AddOnPointerExitListener(UnityAction action)
    {
        OnButtonPointerExit += () => action();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnButtonPointerEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnButtonPointerExit();
    }
}
