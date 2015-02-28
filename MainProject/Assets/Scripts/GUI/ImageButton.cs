using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ImageButton : MonoBehaviour 
{
    [SerializeField]
    private Button button;
    public Button Button
    {
        get { return button; }
    }

    [SerializeField]
    private Image image;

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


}
