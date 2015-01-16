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
        this.shipControlInterface = shipControlInterface;
    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");

        yield return new WaitForSeconds(1.5f);
    }
    #endregion PublicMethods
    #endregion Methods
}
