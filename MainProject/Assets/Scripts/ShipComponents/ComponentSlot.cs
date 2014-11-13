using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentSlot : MonoBehaviour 
{
    public int index;
    
    private ShipComponent installedComponent = null;
    public ShipComponent InstalledComponent
    {
        get { return installedComponent; }
        set { installedComponent = value; }
    }
	
}
