/*
  ShipComponent.cs
  Mission: Invasion
  Created by Rohun Banerji on Nov 12/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
#endregion Usings

public enum ComponentType { Weapon, Defense, Engineering, Support }

[Serializable]
public abstract class ShipComponent : MonoBehaviour , IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    #region Fields
    //Component info and stats

    private AI_Fleet.PlacementType placement;
    public AI_Fleet.PlacementType Placement
    {
        get { return placement; }
        set { placement = value; }
    }
    [SerializeField]
    private ComponentType compType;
    public ComponentType CompType
    {
        get { return compType; }
    }
    public string componentName;
    public bool unlocked;
    
    [SerializeField]
    private float activationCost;
    public float ActivationCost
    {
        get { return activationCost; }
    }
    [SerializeField]
    private float powerDrain;
    public float PowerDrain
    {
        get { return powerDrain; }
    }
    [SerializeField]
    private float maxHP;
    
    private float compHP;
    public float CompHP
    {
        get { return compHP; }
    }
    [SerializeField]
    private bool canActivate;
    public bool CanActivate
    {
        get { return canActivate; }
    }

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
    [SerializeField]
    private GameObject selectionHalo;
    [SerializeField]
    private Slider hpBar;
    [SerializeField]
    private Sprite singleSprite;
    public Sprite SingleSprite
    {
        get { return singleSprite; }
    }
    [SerializeField]
    private Sprite multipleSprite;
    public Sprite MultipleSprite
    {
        get { return multipleSprite; }
    }
    [SerializeField]
    private string tooltipText;
    public string TooltipText
    {
        get { return tooltipText; }
    }
    

    public TurnBasedUnit ParentShip { get; private set; }

    public delegate void ComponentClickEvent(ShipComponent component);
    public event ComponentClickEvent OnComponentClicked = new ComponentClickEvent((ShipComponent) => { });
    public delegate void ComponentHoverMouseOver(ShipComponent component);
    public event ComponentHoverMouseOver OnComponentMouseOver = new ComponentHoverMouseOver((ShipComponent) => {  });
    public delegate void ComponentPointerExit(ShipComponent component);
    public event ComponentPointerExit OnComponentPointerExit = new ComponentPointerExit((ShipComponent) => { });


    #endregion Fields

    #region Methods

    /// <summary>
    /// Called when a component is clicked on. Raises the OnComponentClicked event.
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        #if FULL_DEBUG
        //Debug.Log("At component: Clicked on component " + componentName);
        #endif
        //Selected = !Selected;
        OnComponentClicked(this);
    }
    /// <summary>
    /// Called when a component is moused over. Raises the OnComponentMouseOver event.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entered component " + componentName); 
        OnComponentMouseOver(this);
    }
    /// <summary>
    /// Called when a pointer leaves a component. Raises the OnComponentPointerExit event.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnComponentPointerExit(this);
    }

    public virtual void Init(TurnBasedUnit parentShip)
    {
        ParentShip = parentShip;
        compHP = maxHP;

    }
    /// <summary>
    /// The component takes the specified amount of damage. Starts the destroy routine if HP reaches 0
    /// </summary>
    /// <param name="_amountOfDamage"></param>
    /// <returns></returns>
    public IEnumerator TakeDamage(float _amountOfDamage)
    {

        compHP -= _amountOfDamage;
        hpBar.value -= _amountOfDamage/maxHP;
        #if FULL_DEBUG
        Debug.Log(componentName+ " takes "+ _amountOfDamage+" damage. Remaining HP: " + compHP);
        #endif
        if (compHP <= 0)
        {
            yield return StartCoroutine(Destroy());
        }
    }
    /// <summary>
    /// Just sets the gameobject to inactive for now
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Destroy()
    {
        Debug.Log(componentName + " Destroyed");

        gameObject.SetActive(false);

        yield return null;
    }
    #endregion Methods
}
