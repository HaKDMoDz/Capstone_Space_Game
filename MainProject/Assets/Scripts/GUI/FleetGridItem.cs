/*
  FleetGridItem.cs
  Mission: Invasion
  Created by Rohun Banerji on March 07, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class FleetGridItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public int Index;

    //Events
    public delegate void PointerClickEvent(FleetGridItem gridItem);
    public event PointerClickEvent OnGridPointerClick = new PointerClickEvent((FleetGridItem) => { });
    public delegate void PointerEnterEvent(FleetGridItem gridItem);
    public event PointerEnterEvent OnGridPointerEnter = new PointerEnterEvent((FleetGridItem) => { });
    public delegate void PointerExitEvent(FleetGridItem gridItem);
    public event PointerExitEvent OnGridPointerExit = new PointerExitEvent((FleetGridItem) => { });

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Pointer Enter: Grid Item " + Index);
        OnGridPointerEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer Exit: Grid Item " + Index);
        OnGridPointerExit(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Pointer Click: Grid Item " + Index);
        OnGridPointerClick(this);
    }
}
