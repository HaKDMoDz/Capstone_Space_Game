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

    private PlayerShip_Old targetShip;

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


    PlayerShip_Old targetPlayer;

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

        targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
        ShipComponent targetComponent = TargetComponent(targetPlayer);

        //move phase
        Move(targetPlayer, targetComponent.Placement);
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
            activeComponents = components;

            trans.LookAt(targetPlayer.transform);
            yield return StartCoroutine(ai_Attack.Attack(targetComponent, damagePerAttack, activeComponents));
            
            receivedAttackCommand = false;
            #if FULL_DEBUG
            Debug.Log(name + "- Attack end");
            #endif
        }
        PostTurnActions();
    }

    public void RetargetNewComponent()
    {
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        activeComponents = components;
        ShipComponent targetComponent = TargetComponent(targetPlayer);
        StartCoroutine(ai_Attack.Attack(targetComponent, damagePerAttack, activeComponents));
    }

    protected override void PostTurnActions()
    {
        
    }

    public void Move(PlayerShip_Old targetPlayer, AI_Fleet.PlacementType _placement)
    {
        if (!receivedMoveCommand)
        {
            #if FULL_DEBUG
            Debug.Log("Move command received " + ShipBPMetaData.BlueprintName);
            #endif
            Vector3 enemyPosition = targetPlayer.transform.position;
            switch (_placement)
            {
                case AI_Fleet.PlacementType.FORWARD:
                    
                    shipMove.destination = enemyPosition + (targetPlayer.transform.forward * range);
                    break;
                case AI_Fleet.PlacementType.AFT:
                    shipMove.destination = enemyPosition + (-targetPlayer.transform.forward * range);
                    break;
                case AI_Fleet.PlacementType.PORT:
                    shipMove.destination = enemyPosition + (-targetPlayer.transform.right * range);
                    break;
                case AI_Fleet.PlacementType.STARBOARD:
                    shipMove.destination = enemyPosition + (targetPlayer.transform.right * range);
                    break;
                case AI_Fleet.PlacementType.COUNT:
                default:
                    break;
            }
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

    private PlayerShip_Old TargetEnemy(List<PlayerShip_Old> playerShips)
    {
        if (playerShips == null || playerShips.Count == 0)
        {
            return null;
        }
        else
        {
            if (targetShip == null)
            {
                Vector3 selfPos = trans.position;
                
                //determine which enemy to target: strongest, weakest, closest, furthest
                float confidence = Random.Range(0.0f, 1.0f);

                //case 1 ... closeset
                if (confidence > 1 - AIManager.tgtClosest)
                {
                    //closest enemy
                    return playerShips.Aggregate((current, next) => Vector3.Distance(current.transform.position, selfPos) < Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }

                //case 2 ... farthest
                if (confidence > 1 - AIManager.tgtClosest + AIManager.tgtFarthest)
                {
                    //farthest enemy
                    return playerShips.Aggregate((current, next) =>Vector3.Distance(current.transform.position, selfPos) > Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }

                //case 3 ... strongest
                if (confidence > 1 - AIManager.tgtClosest + AIManager.tgtFarthest + AIManager.tgtStrongest)
                {
                    //strongest enemy
                    return playerShips.Aggregate((current, next) => current.HullHP > next.HullHP ? current : next);
                }

                //assume case 4... weakest enemy

                //weakest enemy
                return playerShips.Aggregate((current, next) => current.HullHP < next.HullHP ? current : next);
            }

            //target ship wasn't null so return your current target
            return targetShip;
        }     
    }

    private void targetCompConfLevel1(PlayerShip_Old _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);

        //setup raytrace
        float distanceToTarget;
        RaycastHit hit;

        Debug.DrawRay(trans.position + ComponentGridTrans.position, ((_ship.transform.position + _ship.ComponentGridTrans.position) - (trans.position + ComponentGridTrans.position)), Color.red, 10.0f, false);
        if (Physics.Raycast(transform.position + ComponentGridTrans.position, ((_ship.transform.position + _ship.ComponentGridTrans.position) - (trans.position + ComponentGridTrans.position)), out hit, 1 << TagsAndLayers.ComponentsLayer))
        {
            distanceToTarget = hit.distance;
            Debug.LogError(hit.collider.name + " : " + distanceToTarget);
        }
        _targetComponent = hit.collider.GetComponent<ShipComponent>();
        Debug.LogError(_targetComponent + ":" + _targetComponent.Placement);

        ////go in order of: weapons, defensive, support, engineering
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else
        {
            _targetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }
    }

    private void targetCompConfLevel2(PlayerShip_Old _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 2: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private void targetCompConfLevel3(PlayerShip_Old _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 3: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private void targetCompConfLevel4(PlayerShip_Old _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 4: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private void targetCompConfLevel5(PlayerShip_Old _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 5: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private ShipComponent TargetComponent(PlayerShip_Old _ship)
    {
        ShipComponent _targetComponent = null ;

        //float confidence = Random.Range(0.0f, 1.0f);
        float confidence = 0.15f;


        //get count for player fleet
        // get count for enemy fleet

        //calculate basis for confidence

        //adjust based on archetypes

        //for now I'm calling the line of confidence at 0.5 this will change as the AI gets more complex -A

        //setup a 5 step system ranging from dullard with a club to military commander with laser guided intel
        //set confidence to choose from those 5 options
        //Debug.LogWarning(confidence);

        float confLevel1Cutoff = 0.2f;
        float confLevel2Cutoff = 0.4f;
        float confLevel3Cutoff = 0.6f;
        float confLevel4Cutoff = 0.8f;

        if (confidence < confLevel1Cutoff)
        {
            Debug.LogWarning("AI_Ship: Targeting component: level 1");
            targetCompConfLevel1(_ship, out _targetComponent);
        }
        else if (confidence < confLevel2Cutoff)
        {
            Debug.LogWarning("AI_Ship: Targeting component: level 2");
            targetCompConfLevel2(_ship, out _targetComponent);
        }
        else if (confidence < confLevel3Cutoff)
        {
            Debug.LogWarning("AI_Ship: Targeting component: level 3");
            targetCompConfLevel3(_ship, out _targetComponent);
        }
        else if (confidence < confLevel4Cutoff)
        {
            Debug.LogWarning("AI_Ship: Targeting component: level 4");
            targetCompConfLevel4(_ship, out _targetComponent);
        }
        else 
        {
            Debug.LogWarning("AI_Ship: Targeting component: level 5");
            targetCompConfLevel5(_ship, out _targetComponent);
        }

        //if (confidence >= 0.5f)
        //{
        //    //go in order of: weapons, defensive, support, engineering
        //    if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        //    {
        //       _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }

        //    //Debug.LogError(_targetComponent);

        //    if (_targetComponent == null)
        //    {
        //        Debug.LogError("Something is very wrong. all component lists empty. AI cannot target components on an empty ship");
        //    }
        //}
        //else
        //{
        //    // go in order of: engineering, support, weapons, defensive
        //    if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        //    {
        //       _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }
        //    else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        //    {
        //        _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        //    }

           // Debug.LogError(_targetComponent);

            if (_targetComponent == null)
            {
                Debug.LogError("Something is very wrong. all component lists empty. AI cannot target components on an empty ship");
            }
       // }

        return _targetComponent;
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
