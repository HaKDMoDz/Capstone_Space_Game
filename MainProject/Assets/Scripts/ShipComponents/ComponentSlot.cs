using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentSlot : MonoBehaviour
{
    #region Fields

    public int index; //assigned in the prefab in advance when the component grid is created for a given ship model
    [SerializeField]
    private AI_Fleet.PlacementType placement;
    public AI_Fleet.PlacementType Placement
    {
        get { return placement; }
        set { placement = value; }
    }

    private ShipComponent installedComponent = null;
    public ShipComponent InstalledComponent
    {
        get { return installedComponent; }
        set { installedComponent = value;
        Debug.Log("Set installed component " + value);
        }
    }

    #endregion Fields

}
