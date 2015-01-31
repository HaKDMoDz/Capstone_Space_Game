using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Ship : TurnBasedUnit 
{
    #region Fields
    #region Internal
    private bool receivedMoveCommand;
    private bool receivedAttackCommand;

    //TEMP
    private float range = 50.0f;
    //TEMP
    private float damagePerAttack = 35.0f;

    public AI_Attack ai_Attack { get; private set; }

    List<ShipComponent> activeComponents = new List<ShipComponent>();

    #endregion Internal
    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, AI_Attack ai_Attack)
    {
        base.Init(shipBP, shipMove);
        this.ai_Attack = ai_Attack;

        foreach (ShipComponent component in shipBP.slot_component_table.Values)
        {
            activeComponents.Add(component);
            component.Init();
        }

    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
#if FULL_DEBUG
        Debug.Log("AI unit turn");
#endif
        // AI doesn't wait for space to be pressed...

        //move phase
        Move();
        if (receivedMoveCommand)
        {
            yield return StartCoroutine(shipMove.Move());
            receivedMoveCommand = false;
#if FULL_DEBUG
            Debug.Log(shipBPMetaData.blueprintName + "- Movement end");
#endif
        }

        //attack phase
        Attack();

        if (receivedAttackCommand)
        {
            TurnBasedUnit targetEnemy = TargetEnemy(TurnBasedCombatSystem.Instance.units);
            //yield return StartCoroutine(ai_Attack.Attack(targetEnemy, damagePerAttack));

            yield return StartCoroutine(ai_Attack.Attack(targetEnemy, damagePerAttack, activeComponents));
            receivedAttackCommand = false;
#if FULL_DEBUG
            Debug.Log(shipBPMetaData.blueprintName + "- Attack end");
#endif
        }
        yield return null;
    }

    public void Move()
    {
        if (!receivedMoveCommand)
        {
#if FULL_DEBUG
            Debug.Log("Move command received " + shipBPMetaData.blueprintName);
#endif
            Vector3 enemyPosition = TargetEnemy(TurnBasedCombatSystem.Instance.units).transform.position;
            Vector3 directionBetween = (transform.position - enemyPosition).normalized;
            shipMove.destination = enemyPosition + (directionBetween * range);
            receivedMoveCommand = true;
        }
    }
    public void Attack()
    {
        if (!receivedAttackCommand)
        {
#if FULL_DEBUG
            Debug.Log("Attack command received " + shipBPMetaData.blueprintName);
#endif
            receivedAttackCommand = true;
        }
    }

 private TurnBasedUnit TargetEnemy(List<TurnBasedUnit> _units)
{
    List<TurnBasedUnit> currentTargets = new List<TurnBasedUnit>();
    
    foreach (TurnBasedUnit unit in _units)
    {
        if (unit is PlayerShip)
        {
            currentTargets.Add(unit);
        }
    }

     //insert logic here to determine closest enemy

    if (currentTargets.Count > 0)
    {
        return currentTargets[0];
    }
    else
    {
        return null;
        //flag turnBasedcontroller... the battle is OVER
    }
    
}

    #endregion PublicMethods
    #endregion Methods
}
