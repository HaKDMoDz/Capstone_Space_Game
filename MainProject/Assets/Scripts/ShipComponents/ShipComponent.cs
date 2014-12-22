using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

public abstract class ShipComponent : MonoBehaviour 
{

    [SerializeField]
    public ComponentType CompType { get; private set; }
    
    public int ID;
    public string componentName;
    public bool unlocked;
    public float activationCost;
    public float powerDrain;

}
