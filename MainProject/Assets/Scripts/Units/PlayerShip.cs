using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    public ShipControlInterface shipControlInterface { get; private set; }

    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, ShipControlInterface shipControlInterface)
    {
        base.Init(shipBP, shipMove);
        #if FULL_DEBUG
        Debug.Log("PlayerShip Init");
	    #endif
        
        this.shipControlInterface = shipControlInterface;
    }
    #endregion PublicMethods
    #endregion Methods
}
