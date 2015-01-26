using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Ship : TurnBasedUnit 
{
    #region Fields
    #region Internal
    private bool receivedMoveCommand;

    //TEMP
    private float range = 100.0f;

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

        Move();
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
            List<TurnBasedUnit> currentTargets = new List<TurnBasedUnit>();
            foreach (TurnBasedUnit unit in TurnBasedCombatSystem.Instance.units)
            {
                if (unit is PlayerShip)
                {
                    currentTargets.Add(unit);
                }
            }

            Vector3 enemyPosition = currentTargets[0].transform.position;
            Vector3 directionBetween = (transform.position - enemyPosition).normalized;
            shipMove.destination = enemyPosition + (directionBetween * range);
            receivedMoveCommand = true;
        }
    }

    #endregion PublicMethods
    #endregion Methods
}
