using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

//[Serializable]
public class ShipComponent : MonoBehaviour 
{
    public int ID;
    public string componentName;
    public bool unlocked;

    
    [SerializeField]
    private ComponentType compType;
    public ComponentType CompType
    {
        get { return compType; }
    }
    
}
