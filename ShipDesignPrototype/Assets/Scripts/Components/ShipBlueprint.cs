using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipBlueprint
{

    #region Fields
    Hull hull;
    public Hull Hull
    {
        get { return hull; }
    }
    List<ComponentSlot> componentGrid;

    private Dictionary<ComponentSlot, ShipComponent> componentTable;
    public Dictionary<ComponentSlot, ShipComponent> ComponentTable
    {
        get { return componentTable; }
    }

    public ShipBlueprint(Hull _hull)
    {
        componentTable = new Dictionary<ComponentSlot, ShipComponent>();
        hull = _hull;
        componentGrid = hull.EmptyComponentGrid;
    }

    #endregion

    #region Methods

    public void AddComponent(ShipComponent component, ComponentSlot slot)
    {

    }
    public void RemoveComponent(ShipComponent component, ComponentSlot slot)
    {

    }
    #endregion

}
