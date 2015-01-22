using UnityEngine;
using System.Collections;

public class AI_Ship : TurnBasedUnit 
{
    #region Fields
    #region Internal
    private bool receivedMoveCommand;
    #endregion Internal
    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove)
    {
        base.Init(shipBP, shipMove);
    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("AI unit turn");

        // AI doesn't wait for space to be pressed...

        if (receivedMoveCommand)
        {
            yield return StartCoroutine(shipMove.Move());
            receivedMoveCommand = false;
#if FULL_DEBUG
            Debug.Log(shipBPMetaData.blueprintName + "- Movement end");
#endif
        }
        yield return null;
        
    }

    public void Move()
    {
        if (!receivedMoveCommand)
        {
            Debug.Log("Move command recieved " + shipBPMetaData.blueprintName);
            shipMove.destination = new Vector3(0, 0, 0);
            receivedMoveCommand = true;
        }
    }

    #endregion PublicMethods
    #endregion Methods
}
