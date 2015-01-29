using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

public abstract class ShipComponent : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler
{

    //Component info and stats
    [SerializeField]
    private ComponentType compType;
    public ComponentType CompType
    {
        get { return compType; }
    }
    public int ID;
    public string componentName;
    public bool unlocked;
    public float activationCost;
    public float powerDrain;

    //interface
    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            //selection effect here
            #if FULL_DEBUG
            if(!selectionHalo)
            {
                Debug.LogError("Selection Halo not set");
            }
            #endif
            selectionHalo.SetActive(value);
        }
    }

    //cached references
    private GameObject selectionHalo;

    public delegate void ComponentClickEvent(ShipComponent component);
    public event ComponentClickEvent OnComponentClicked = new ComponentClickEvent((ShipComponent) => { });
    public delegate void ComponentHoverMouseOver(ShipComponent component);
    public event ComponentHoverMouseOver OnComponentMouseOver = new ComponentHoverMouseOver((ShipComponent) => {  });

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on component " + componentName);
        //Selected = !Selected;
        OnComponentClicked(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entered component " + componentName); 
        OnComponentMouseOver(this);
    }


    public virtual void Init()
    {
        selectionHalo = transform.FindChild("SelectionHalo").gameObject;
    }
}
