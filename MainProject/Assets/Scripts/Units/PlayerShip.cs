using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShip : MonoBehaviour
{
    #region Fields

    //references
    public ShipBlueprint shipBP { get; private set; }
    public ShipMove shipMove { get; private set; }
    public ShipControlInterface shipControlInterface { get; private set; }

    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, ShipControlInterface shipControlInterface)
    {
        this.shipBP = shipBP;
        this.shipMove = shipMove;
        this.shipControlInterface = shipControlInterface;
    }
    #endregion PublicMethods
    #endregion Methods
}
