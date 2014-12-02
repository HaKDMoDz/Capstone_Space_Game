using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

public class ShipComponent : MonoBehaviour 
{
    [SerializeField]
    private ComponentType compType;
    public ComponentType CompType
    {
        get { return compType; }
    }

    public string componentName;
    public int ID;
    public bool unlocked;
}
