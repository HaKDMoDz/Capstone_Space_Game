using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    //references
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

        //foreach (ShipComponent component in shipBP.slot_component_table.Values)
        //{
        //    activeComponents.Add(component);
        //    component.Init();
        //}

        trans = transform;

    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        #if FULL_DEBUG
        Debug.Log("AI unit turn");
        #endif
        // AI doesn't wait for space to be pressed...

        PlayerShip targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
        //move phase
        Move(targetPlayer);
        if (receivedMoveCommand)
        {
            yield return StartCoroutine(shipMove.Move());
            receivedMoveCommand = false;
            #if FULL_DEBUG
            Debug.Log(name + "- Movement end");
            #endif
        }

        //attack phase
        Attack();

        if (receivedAttackCommand)
        {
            yield return StartCoroutine(ai_Attack.Attack(targetPlayer, damagePerAttack, activeComponents));
            receivedAttackCommand = false;
            #if FULL_DEBUG
            Debug.Log(name + "- Attack end");
            #endif
        }
        yield return null;
    }

    public void Move(PlayerShip targetPlayer)
    {
        if (!receivedMoveCommand)
        {
            #if FULL_DEBUG
            Debug.Log("Move command received " + shipBPMetaData.blueprintName);
            #endif
            Vector3 enemyPosition = targetPlayer.transform.position;
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

    private PlayerShip TargetEnemy(List<PlayerShip> playerShips)
    {
        if (playerShips == null || playerShips.Count == 0)
        {
            return null;
        }
        else
        {
            //insert logic here to determine closest enemy
            Vector3 selfPos = trans.position;
            return playerShips
                .Aggregate((current, next) =>
                    Vector3.Distance(current.transform.position, selfPos)
                        < Vector3.Distance(next.transform.position, selfPos)
                        ? current : next);
        }
    }

    #endregion PublicMethods
    #endregion Methods
}
