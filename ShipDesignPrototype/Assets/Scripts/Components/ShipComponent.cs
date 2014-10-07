using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class ShipComponent : MonoBehaviour 
{
    public int ID;
    public string componentName;
    public bool unlocked;

    public enum ComponentType { Weapon, Defense, Power, Support}
    [SerializeField]
    private ComponentType compType;

    public ComponentType CompType
    {
        get { return compType; }
    }
    
}
