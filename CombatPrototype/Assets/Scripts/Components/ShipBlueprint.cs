using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShipBlueprint : MonoBehaviour 
{
    public List<ShipComponent> components;

    public List<Component_Weapon> weapons;



    public void Init()
    {
        weapons = components.OfType<Component_Weapon>().ToList();
        foreach (ShipComponent comp in components)
        {
            comp.Init();
        }
    }

}
