using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    public ShipControlInterface shipControlInterface { get; private set; }

    #region Internal
    private bool receivedMoveCommand;
    #endregion Internal
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

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if(receivedMoveCommand)
            {
                yield return StartCoroutine(shipMove.Move());
            }
            yield return null;  
        }
        //yield return new WaitForSeconds(1.5f);
    }

    public void Move(Vector3 destination)
    {
        Debug.Log("Move command recieved " + shipBPMetaData.blueprintName);
        shipMove.destination = destination;
        receivedMoveCommand = true;
    }

    #endregion PublicMethods
    #endregion Methods
}
