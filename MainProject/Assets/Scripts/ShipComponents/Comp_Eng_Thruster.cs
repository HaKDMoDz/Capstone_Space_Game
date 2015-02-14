using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Comp_Eng_Thruster : ShipComponent
{
    [SerializeField]
    private float thrust;
    public float Thrust
    {
        get { return thrust; }
    }
}