using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI_Ship : TurnBasedUnit, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Fields
    
    private bool receivedMoveCommand;
    private bool receivedAttackCommand;

    //TEMP
    private float range = 50.0f;
    //TEMP
    private float damagePerAttack = 35.0f;

    //references
    public AI_Attack ai_Attack { get; private set; }

    List<ShipComponent> activeComponents = new List<ShipComponent>();

    //Events
    public delegate void ShipClickEvent(AI_Ship ship);
    public event ShipClickEvent OnShipClick = new ShipClickEvent((AI_Ship) => { });
    public delegate void ShipMouseEnterEvent(AI_Ship ship);
    public event ShipMouseEnterEvent OnShipMouseEnter = new ShipMouseEnterEvent((AI_Ship) => { });
    public delegate void ShipMouseExitEvent(AI_Ship ship);
    public event ShipMouseExitEvent OnShipMouseExit = new ShipMouseExitEvent((AI_Ship) => { });

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

    protected override void PreTurnActions()
    {
        
    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        #if FULL_DEBUG
        Debug.Log("AI unit turn");
        #endif

        PreTurnActions();

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
        PostTurnActions();
    }

    protected override void PostTurnActions()
    {
        
    }

    public void Move(PlayerShip targetPlayer)
    {
        if (!receivedMoveCommand)
        {
            #if FULL_DEBUG
            Debug.Log("Move command received " + ShipBPMetaData.BlueprintName);
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
            Debug.Log("Attack command received " + ShipBPMetaData.BlueprintName);
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

    #region InternalCallbacks
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse over " + name);
        OnShipMouseEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exit " + name);
        OnShipMouseExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse click " + name);
        OnShipClick(this);
    }
    #endregion InternalCallbacks
}
