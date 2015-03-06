/*
  ImageButton.cs
  Mission: Invasion
  Created by Rohun Banerji on Feb 28/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button;
    public Button Button
    {
        get { return button; }
    }

    [SerializeField]
    private Image image;

    private delegate void ButtonPointerEnterEvent();
    private event ButtonPointerEnterEvent OnButtonPointerEnter = new ButtonPointerEnterEvent(() => { });
    private delegate void ButtonPointerExitEvent();
    private event ButtonPointerExitEvent OnButtonPointerExit = new ButtonPointerExitEvent(() => { });

    public void SetImage(Sprite image)
    {
#if FULL_DEBUG
        if(image == null)
        {
            Debug.LogError("Image is null");
            return;
        }
#endif
        this.image.sprite = image;
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
        OnButtonPointerEnter += () => action();
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
