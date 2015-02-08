using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentSlot : MonoBehaviour
{
    #region Fields

    public int index;

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
        set { installedComponent = value; }
    }

    #endregion Fields

}
