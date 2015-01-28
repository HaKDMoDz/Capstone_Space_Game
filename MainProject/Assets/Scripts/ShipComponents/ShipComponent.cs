using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

public abstract class ShipComponent : MonoBehaviour , IPointerClickHandler
{

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

    bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            //selection effect here
            transform.FindChild("SelectionHalo").gameObject.SetActive(value);

        }
    }


    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on component " + componentName);
        Selected = !Selected;
    }
}
