﻿using UnityEngine;
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
    //private float range = 50.0f;
    //TEMP
    //private float damagePerAttack = 35.0f;

    private float frontalAssaultRange = 50.0f;
    private Component_Weapon laser;
    public float laserRange;
    private Component_Weapon missile;
    public float missileRange;
    private Component_Weapon railgun;
    public float railgunRange;

    private PlayerShip targetShip;

    private float range;

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


    PlayerShip targetPlayer;

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

        laser = ComponentTable.id_comp_table.Where(c => c.Value.CompSpecificType == ComponentSpecificType.LASER).ToList().First().Value as Component_Weapon;
        missile = ComponentTable.id_comp_table.Where(c => c.Value.CompSpecificType == ComponentSpecificType.MISSILE).ToList().First().Value as Component_Weapon;
        railgun = ComponentTable.id_comp_table.Where(c => c.Value.CompSpecificType == ComponentSpecificType.MASS_D).ToList().First().Value as Component_Weapon;

        //Debug.Log(laser);
        laserRange = laser.range;
        missileRange = missile.range;
        railgunRange = railgun.range;

        range = frontalAssaultRange;
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
        targetPlayer.ShowHPBars(true);
        if (targetComponent != null)
        {
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
                while ( targetPlayer.HullHP > 0 && CurrentPower > activeComponents.Where(c => c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.ActivationCost < next.ActivationCost ? curr : next).ActivationCost)
                {
                    Debug.LogWarning(targetPlayer.HullHP);
                    yield return StartCoroutine(ActivateWeapons(targetPlayer, targetComponent));
                    yield return new WaitForSeconds(0.2f);
                    targetComponent = TargetComponent(targetPlayer);
                }
                receivedAttackCommand = false;
#if FULL_DEBUG
                Debug.Log(name + "- Attack end");
#endif
            }
        }
        PostTurnActions();
    }

    public void RetargetNewComponent()
    {
        activeComponents = components;
        if (targetPlayer != null)
        {
            ShipComponent targetComponent = TargetComponent(targetPlayer);
        }
        //StartCoroutine(ai_Attack.Attack(targetComponent, damagePerAttack, activeComponents));
    }

    public void RetargetNewShip()
    {
        targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
    }

    protected override void PostTurnActions()
    {
        if (targetShip)
	    {
            targetShip.ShowHPBars(false);
	    }
        
    }

    public void Move(PlayerShip targetPlayer, AI_Fleet.PlacementType _placement)
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

                    shipMove.destination = enemyPosition + (targetPlayer.transform.forward * (range - 10));
                    break;
                case AI_Fleet.PlacementType.AFT:
                    shipMove.destination = enemyPosition + (-targetPlayer.transform.forward * (range - 10));
                    break;
                case AI_Fleet.PlacementType.PORT:
                    shipMove.destination = enemyPosition + (-targetPlayer.transform.right * (range - 10));
                    break;
                case AI_Fleet.PlacementType.STARBOARD:
                    shipMove.destination = enemyPosition + (targetPlayer.transform.right * (range - 10));
                    break;
                case AI_Fleet.PlacementType.COUNT:
                default:
                    break;
            }
            receivedMoveCommand = true;

            float moveDistance = Vector3.Distance(shipMove.destination, trans.position);
            float movePowerCost = Mathf.Round(moveDistance * MoveCost);

            if (CurrentPower - movePowerCost >= 0)
            {
                CurrentPower = CurrentPower - movePowerCost;
            }
            else
            {
                shipMove.destination = trans.position;
            }

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
            if (targetShip == null)
            {
                Vector3 selfPos = trans.position;

                float confidence = 0;

                //get count for player fleet
                int playerFleetStrength = GameObject.FindObjectsOfType<PlayerShip>().Count();
                // get count for enemy fleet
                int aiFleetStrength = GameObject.FindObjectsOfType<AI_Ship>().Count();

                //calculate basis for confidence

                int totalShips = playerFleetStrength + aiFleetStrength;

                confidence = (float)aiFleetStrength / (float)totalShips;
                //adjust based on ship and target

                //case 1 ... closest enemy
                if (confidence < AIManager.tgtClosest) //closest enemy
                {
                    return playerShips.Aggregate((current, next) => Vector3.Distance(current.transform.position, selfPos) < Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }
                else if (confidence < AIManager.tgtWeakest) //weakest enemy
                {  
                return playerShips.Aggregate((current, next) => (current.HullHP + current.ShieldStrength) <= (next.HullHP + next.ShieldStrength) ? current : next); 
                }
                else if (confidence > 1 - AIManager.tgtClosest + AIManager.tgtFarthest + AIManager.tgtStrongest)
                {
                    //farthest enemy
                return playerShips.Aggregate((current, next) => Vector3.Distance(current.transform.position, selfPos) > Vector3.Distance(next.transform.position, selfPos) ? current : next);
                }

                //assume case 4... strongest
                return playerShips.Aggregate((current, next) => (current.HullHP + current.ShieldStrength) >= (next.HullHP + next.ShieldStrength) ? current : next);
            }

            //target ship wasn't null so return your current target
            return targetShip;
        }
    }

    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Vector3 componentGridPos = trans.position + ComponentGridTrans.position;
        Vector3 targetCompPos = component.transform.position;
        Vector3 directionToTargetComp = targetCompPos - componentGridPos;
        //Ray ray = new Ray(componentGridPos, targetCompPos - componentGridPos);
        RaycastHit[] hits = Physics.RaycastAll(componentGridPos, directionToTargetComp, GlobalVars.RayCastRange);
#if FULL_DEBUG
        if (hits == null || hits.Length == 0) Debug.LogError("No raycast hits");
#endif
        List<ShipComponent> hitComponents = new List<ShipComponent>();
        foreach (RaycastHit hit in hits)
        {
            ShipComponent comp = hit.collider.GetComponent<ShipComponent>();
            if (comp && comp.ParentShip != this)
            {
                hitComponents.Add(comp);
            }
        }
        ShipComponent closestComp = null;

        if (hitComponents.Count > 0)
        {
          closestComp = hitComponents
            .Select(c => c.transform)
            .Aggregate((curr, next) =>
                Vector3.Distance(curr.position, componentGridPos)
                < Vector3.Distance(next.position, componentGridPos)
                ? curr : next)
            .GetComponent<ShipComponent>();  
        }
        
        
        return closestComp;
    }

    private void targetCompConfLevel1(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);


        //select range based on longest range of component
        if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf).Count() > 0)
        {
            range = railgunRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf).Count() > 0)
        {
            range = missileRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf).Count() > 0)
        {
            range = laserRange;
        }
        else
        {
            range = frontalAssaultRange;
        }


        ShipComponent idealTargetComponent = null;
        //go in order of: weapons, defensive, support, engineering
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);

        }
        else
        {
            idealTargetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        if (idealTargetComponent == null)
        {
            idealTargetComponent = TargetComponent(targetShip);
        }
        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);

    }


    private void targetCompConfLevel2(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);

        //select range based on shortest range of component
        if (components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf).Count() > 0)
        {
            range = laserRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf).Count() > 0)
        {
            range = missileRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf).Count() > 0)
        {
            range = railgunRange;
        }
        else
        {
            range = frontalAssaultRange;
        }


        ShipComponent idealTargetComponent = null;
        //go in order of: weapons, defensive, support, engineering
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);

        }
        else
        {
            idealTargetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);

    }

    private void targetCompConfLevel3(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);
        ShipComponent idealTargetComponent = null;

        range = frontalAssaultRange;

        //go in order of: weapons, defensive, support, engineering
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);

        }
        else
        {
            idealTargetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);
    }

    private void targetCompConfLevel4(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        range = frontalAssaultRange;

        //make lists of components by placement
        List<ShipComponent> fwdComponents = new List<ShipComponent>();
        fwdComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.FORWARD && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> aftComponents = new List<ShipComponent>();
        aftComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.AFT && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> portComponents = new List<ShipComponent>();
        portComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.PORT && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> starComponents = new List<ShipComponent>();
        starComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.STARBOARD && c.gameObject.activeSelf).ToList<ShipComponent>();

        Debug.Log("fwd comp: " + fwdComponents.Count());
        Debug.Log("aft comp: " + aftComponents.Count());
        Debug.Log("port comp: " + portComponents.Count());
        Debug.Log("star comp: " + starComponents.Count());

        //determine list with least defense
        int fwdArmour, aftArmour, portArmour, starArmour;

        fwdArmour = fwdComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        aftArmour = aftComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        portArmour = portComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        starArmour = starComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;

        Debug.Log("fwd arm: " + fwdArmour);
        Debug.Log("aft arm: " + aftArmour);
        Debug.Log("port arm: " + portArmour);
        Debug.Log("star arm: " + starArmour);

        //target from that side
        if (fwdArmour <= aftArmour && fwdArmour <= portArmour && fwdArmour <= starArmour)
        {
            //fwd is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 4: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else if (aftArmour <= fwdArmour && aftArmour <= portArmour && aftArmour <= starArmour)
        {
            //aft is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 4: aft:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else if (portArmour <= fwdArmour && portArmour <= aftArmour && portArmour <= starArmour)
        {
            //port is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 4: port:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else
        {
            //starboard is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 4: starboard:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }

        if (_targetComponent == null)
        {
            targetCompConfLevel5(_ship, out _targetComponent);
        }

    }

    private void targetCompConfLevel5(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        range = frontalAssaultRange;

        ShipComponent idealTargetComponent = null;
        //go in order of: engineering, support, weapons, defensive
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            idealTargetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else
        {
            idealTargetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 5: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);

    }

    private float trialShipVsShip(PlayerShip _targetShip)
    {
        // calculate ai and player defenses
        float aiShield = Components.Where(c => c.CompSpecificType == ComponentSpecificType.SHIELD_G).ToArray().Length;
        float aiArmour = Components.Where(c => c.CompSpecificType == ComponentSpecificType.ARMOUR).ToArray().Length;
        float playerShield = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.SHIELD_G).ToArray().Length;
        float playerArmour = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.ARMOUR).ToArray().Length;

        // calculate ai and player excess power
        float aiExcessPower = CurrentPower;
        float playerExcessPower = _targetShip.CurrentPower;

        // calculate ai and player thrusters
        float aiThrusters = Components.Where(c => c.CompSpecificType == ComponentSpecificType.THRUSTER).ToArray().Length;
        float playerThrusters = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.THRUSTER).ToArray().Length;

        // calculate ai and player max hull HP
        float aiMaxHullHP = MaxHullHP;
        float playerMaxHullHP = _targetShip.MaxHullHP;

        // calculate ai and player  max shield
        float aiMaxShield = ShieldStrength;
        float playerMaxShield = _targetShip.ShieldStrength;
        //calculate ai and player weapons vs hull
        ShipComponent[] aiVsHullComponents = Components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D || c.CompSpecificType == ComponentSpecificType.MISSILE).ToArray();
        float aiVsHull = 0;
        foreach (Component_Weapon component in aiVsHullComponents)
        {
            aiVsHull += component.HullDamage;
        }
        ShipComponent[] playerVsHullComponents = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D || c.CompSpecificType == ComponentSpecificType.MISSILE).ToArray();
        float playerVsHull = 0;
        foreach (Component_Weapon component in playerVsHullComponents)
        {
            playerVsHull += component.HullDamage;
        }
        //calculate ai and player weapons vs shield
        ShipComponent[] aiVsShieldComponents = Components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER).ToArray();
        float aiVsShield = 0;
        foreach (Component_Weapon component in aiVsHullComponents)
        {
            aiVsShield += component.ShieldDamage;
        }
        ShipComponent[] playerVsShieldComponents = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER).ToArray();
        float playerVsShield = 0;
        foreach (Component_Weapon component in playerVsHullComponents)
        {
            playerVsShield += component.ShieldDamage;
        }

        float aiHP = 0;
        ShipComponent[] aiHullComponents = Components.Where(c => c.CompSpecificType == ComponentSpecificType.ARMOUR).ToArray();
        foreach (Comp_Def_Armour item in aiHullComponents)
        {
            aiHP += item.CompHP;
        }

        float playerHP = 0;
        ShipComponent[] playerHullComponents = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.ARMOUR).ToArray();
        foreach (Comp_Def_Armour item in playerHullComponents)
        {
            playerHP += item.CompHP;
        }

        //run a simple trial based on these parameters and see who wins

        float runningAdjustment = 0;

        //this is the simplest trial. it's basically punchies.
        //simple rules. 2 players. each player punches the other in the arm
        //whichever player punched lightest looses
        bool keepGoing = true;
        while (keepGoing)
        {
            //run a typical turn

            //simple version. more adv. version to come later
            //take damage from each other
            aiExcessPower = CurrentPower;
            playerExcessPower = _targetShip.CurrentPower;

            // damage the player
            if (playerMaxShield >= 0)
            {
                while (aiExcessPower >= 0)
                {
                    playerMaxShield -= aiVsShield;
                    aiExcessPower -= aiVsShield;
                }
            }
            else
            {
                while (aiExcessPower >= 0)
                {
                    playerHP -= aiVsHull;
                    aiExcessPower -= aiVsHull;
                }
            }

            if (playerHP <= 0)
            {
                keepGoing = false;
                break;
            }

            //damage the ai
            if (aiMaxShield >= 0 && playerHP >= 0)
            {
                while (playerExcessPower >= 0)
                {
                    aiMaxShield -= playerVsShield;
                    playerExcessPower -= playerVsShield;
                }
            }
            else
            {
                while (playerExcessPower >= 0)
                {
                    aiHP -= playerVsHull;
                    playerExcessPower -= playerVsHull;
                }
            }

            if (aiHP <= 0)
            {
                keepGoing = false;
                break;
            }
        }

        if (aiHP <= 0)
        {
            runningAdjustment -= 0.1f;//replace with proper external weighting modifiable by GA
        }

        if (playerHP <= 0)
        {
            runningAdjustment += 0.1f;//replace with proper external weighting modifiable by GA
        }

        //return the confidence adjustment
        return runningAdjustment;
    }

    private ShipComponent TargetComponent(PlayerShip _ship)
    {
        if (_ship == null)
        {
            return null;
        }
        ShipComponent _targetComponent = null;
        float confidence = 0;

        //get count for player fleet
        int playerFleetStrength = GameObject.FindObjectsOfType<PlayerShip>().Count();
        // get count for enemy fleet
        int aiFleetStrength = GameObject.FindObjectsOfType<AI_Ship>().Count();

        //calculate basis for confidence
        int totalShips = playerFleetStrength + aiFleetStrength;

        confidence = (float)aiFleetStrength / (float)totalShips;
        //adjust based on ship and target

        confidence += trialShipVsShip(_ship);

        //setup a 5 step system ranging from kite at max range to target most valuable components first and get to them any way you can
        //set confidence to choose from those 5 options

        float confLevel1Cutoff = 0.3f;
        float confLevel2Cutoff = 0.5f;
        float confLevel3Cutoff = 0.75f;
        float confLevel4Cutoff = 0.9f;

        if (confidence < confLevel1Cutoff)
        {
            targetCompConfLevel1(_ship, out _targetComponent);
        }
        else if (confidence < confLevel2Cutoff)
        {
            targetCompConfLevel2(_ship, out _targetComponent);
        }
        else if (confidence < confLevel3Cutoff)
        {
            targetCompConfLevel3(_ship, out _targetComponent);
        }
        else if (confidence < confLevel4Cutoff)
        {
            targetCompConfLevel4(_ship, out _targetComponent);
        }
        else
        {
            targetCompConfLevel5(_ship, out _targetComponent);
        }

        if (_targetComponent == null)
        {
            Debug.LogWarning("all component lists empty. AI cannot target components on an empty or dead ship. skipping turn");
        }

        return _targetComponent;
    }

    public IEnumerator Attack(ShipComponent _target)
    {
        Debug.LogWarning(_target + " targetted");
        GetComponent<AI_Ship>().CurrentPower = GetComponent<AI_Ship>().MaxPower;
        bool keepFiring = true;

        float distanceToTarget = (_target.transform.position - transform.position).magnitude;

        while (keepFiring)
        {
            if (TurnBasedCombatSystem.Instance.playerShips == null || TurnBasedCombatSystem.Instance.playerShips.Count() <= 0)
            {
                keepFiring = false;
                break;
            }

            if (_target.ParentShip.HullHP <= 0)
            {
                GetComponent<AI_Ship>().RetargetNewShip();
                GetComponent<AI_Ship>().RetargetNewComponent();
            }

            if (_target.ParentShip.ShieldStrength > 0)
            {
                foreach (Comp_Wpn_Laser weapon in components.Where(c => c is Comp_Wpn_Laser))
                {
                    if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0 && distanceToTarget < GetComponent<AI_Ship>().laserRange)
                    {
                        GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                        yield return StartCoroutine(weapon.Fire(_target, () => { }));

                        if (_target.ParentShip.HullHP <= 0)
                        {
                            GetComponent<AI_Ship>().RetargetNewShip();
                            GetComponent<AI_Ship>().RetargetNewComponent();
                        }
                    }
                    else
                    {
                        {
                            keepFiring = false;
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach (Component_Weapon weapon in components.Where(c => c is Comp_Wpn_Missile))
                {
                    if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0 && distanceToTarget < GetComponent<AI_Ship>().missileRange)
                    {
                        GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                        yield return StartCoroutine(weapon.Fire(_target, () => { }));

                        if (_target.ParentShip.HullHP <= 0)
                        {
                            GetComponent<AI_Ship>().RetargetNewShip();
                            GetComponent<AI_Ship>().RetargetNewComponent();
                        }
                    }
                    else
                    {
                        {
                            keepFiring = false;
                        }
                        break;
                    }
                }

                foreach (Component_Weapon weapon in components.Where(c => c is Comp_Wpn_Railgun))
                {
                    if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0 && distanceToTarget < GetComponent<AI_Ship>().railgunRange)
                    {
                        GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                        yield return StartCoroutine(weapon.Fire(_target, () => { }));

                        if (_target.ParentShip.HullHP <= 0)
                        {
                            GetComponent<AI_Ship>().RetargetNewShip();
                            GetComponent<AI_Ship>().RetargetNewComponent();
                        }
                    }
                    else
                    {
                        {
                            keepFiring = false;
                        }
                        break;
                    }
                }
            }

            //fire lasers if all else fails/ or you have power left
            foreach (Comp_Wpn_Laser weapon in components.Where(c => c is Comp_Wpn_Laser))
            {
                if (_target.CompHP > 0 && weapon.PowerDrain <= GetComponent<AI_Ship>().CurrentPower && _target.ParentShip.HullHP > 0 && distanceToTarget < GetComponent<AI_Ship>().laserRange)
                {
                    GetComponent<AI_Ship>().CurrentPower -= weapon.PowerDrain;
                    yield return StartCoroutine(weapon.Fire(_target, () => { }));

                    if (_target.ParentShip.HullHP <= 0)
                    {
                        GetComponent<AI_Ship>().RetargetNewShip();
                        GetComponent<AI_Ship>().RetargetNewComponent();
                    }
                }
                else
                {
                    {
                        keepFiring = false;
                    }
                    break;
                }
            }

            yield return null;
        }
        yield return null;
    }

    private IEnumerator ActivateWeapons(PlayerShip _targetShip, ShipComponent _targetComponent)
    {
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, _targetShip.transform, GlobalVars.CameraAimAtPeriod));

        float distanceToTarget = (_targetShip.transform.position - trans.position).magnitude;

        List<ShipComponent> selectedComponents;
        if (distanceToTarget < laserRange)
        {
            selectedComponents = new List<ShipComponent>();

            if (_targetShip.ShieldStrength > 0)
            {
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf));
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf));
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf));
            }
            else
            {
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf));
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf));
                selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf));
            }
           
        }
        else if (distanceToTarget < missileRange)
        {
             selectedComponents = components.Where(c => (c.CompSpecificType == ComponentSpecificType.MISSILE) && c.gameObject.activeSelf).ToList();
             selectedComponents.AddRange(components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf));
             
        }
        else if (distanceToTarget < railgunRange)
        {
            selectedComponents = components.Where(c => (c.CompSpecificType == ComponentSpecificType.MASS_D) && c.gameObject.activeSelf).ToList();
        }
        else
        {
            selectedComponents = new List<ShipComponent>();
            Debug.LogWarning("AI_Ship::ActivateWeapons() No components can fire at this range");
        }
        

        Component_Weapon[] selectedWeapons = selectedComponents.Cast<Component_Weapon>().ToArray();

        foreach (Component_Weapon item in selectedWeapons)
        {
            Debug.LogWarning(item.CompSpecificType);
        }

        Component_Weapon[] selectedLasers = selectedWeapons.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf).ToArray();
        Component_Weapon[] selectedMissiles = selectedWeapons.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf).ToArray();
        Component_Weapon[] selectedRailguns = selectedWeapons.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf).ToArray();

        float totalPowerUsed = 0;
        bool lasersDone = false;
        bool missilesDone = false;
        bool railgunsDone = false;

        //int numWeaponsToActivate = GetNumWeaponsToActivate(selectedWeapons[0], _targetShip, _targetComponent, selectedWeapons.Length);
        int numLasersToActivate;
        if (selectedLasers.Count() > 0 && _targetComponent)
        {
            numLasersToActivate = GetNumWeaponsToActivate(selectedLasers[0], _targetShip, _targetComponent, selectedLasers.Length);
            totalPowerUsed += (numLasersToActivate * selectedLasers[0].ActivationCost);
        }
        else
        {
            numLasersToActivate = 0;
            lasersDone = true;
            
        }

        int numMissilesToActivate;
        if (selectedMissiles.Count() > 0 && _targetComponent)
        {
            numMissilesToActivate = GetNumWeaponsToActivate(selectedMissiles[0], _targetShip, _targetComponent, selectedMissiles.Length);
            totalPowerUsed += (numMissilesToActivate * selectedMissiles[0].ActivationCost);
        }
        else
        {
            numMissilesToActivate = 0;
            missilesDone = true;
        }

        int numRailgunsToActivate;
        if (selectedRailguns.Count() > 0 && _targetComponent)
        {
            numRailgunsToActivate = GetNumWeaponsToActivate(selectedRailguns[0], _targetShip, _targetComponent, selectedRailguns.Length);
            totalPowerUsed += (numRailgunsToActivate * selectedRailguns[0].ActivationCost);
        }
        else
        {
            numRailgunsToActivate = 0;
            railgunsDone = true;
        }
      
#if FULL_DEBUG
        Debug.Log("ActivateWeapons");
        //Debug.Log("numWeaponsToActivate " + numWeaponsToActivate);
        Debug.Log("numLasersToActivate " + numLasersToActivate);
        Debug.Log("numMissilesToActivate " + numMissilesToActivate);
        Debug.Log("numRailgunsToActivate " + numRailgunsToActivate);
        Debug.Log("num selected weapons: " + selectedWeapons.Length);
#endif
        
        CurrentPower -= totalPowerUsed;
        int weaponHitCounter = 0;

        for (int i = 0; i < numLasersToActivate; i++)
        {
            if (selectedLasers[i].gameObject.activeSelf && _targetComponent.CompHP > 0.0f)
            {
                StartCoroutine(selectedLasers[i].Fire(_targetComponent, () => { weaponHitCounter++; }));
                yield return new WaitForSeconds(Random.Range(PlayerShipConfig.WeaponActivationInterval.x, PlayerShipConfig.WeaponActivationInterval.y));
            }
            else
            {
                weaponHitCounter++;
            }

            if (_targetComponent.CompHP <= 0)
            {
                RetargetNewComponent();
                break;
            }
        }
        while (weaponHitCounter < numLasersToActivate)
        {
            lasersDone = true;
            break;
        }

        weaponHitCounter = 0;
        for (int i = 0; i < numMissilesToActivate; i++)
        {
            if (selectedMissiles[i].gameObject.activeSelf && _targetComponent.CompHP > 0.0f)
            {
                StartCoroutine(selectedMissiles[i].Fire(_targetComponent, () => { weaponHitCounter++; }));
                yield return new WaitForSeconds(Random.Range(PlayerShipConfig.WeaponActivationInterval.x, PlayerShipConfig.WeaponActivationInterval.y));
            }
            else
            {
                weaponHitCounter++;
            }

            if (_targetComponent.CompHP <= 0)
            {
                RetargetNewComponent();
                break;
            }
        }
        while (weaponHitCounter < numMissilesToActivate)
        {
            missilesDone = true;
            break;
        }

        weaponHitCounter = 0;
        for (int i = 0; i < numRailgunsToActivate; i++)
        {
            if (selectedRailguns[i].gameObject.activeSelf && _targetComponent.CompHP > 0.0f)
            {
                StartCoroutine(selectedRailguns[i].Fire(_targetComponent, () => { weaponHitCounter++; }));
                yield return new WaitForSeconds(Random.Range(PlayerShipConfig.WeaponActivationInterval.x, PlayerShipConfig.WeaponActivationInterval.y));
            }
            else
            {
                weaponHitCounter++;
            }

            if (_targetComponent.CompHP <= 0)
            {
                RetargetNewComponent();
                break;
            }
        }
        while (weaponHitCounter < numRailgunsToActivate)
        {
            railgunsDone = true;
            break;
        }

        if (lasersDone && missilesDone && railgunsDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Returns the number of weapon activations required to kill the target component or ship - avoids over-firing
    /// </summary>
    /// <returns></returns>
    private int GetNumWeaponsToActivate(Component_Weapon weapon, PlayerShip _targetShip, ShipComponent _targetComponent, int numSelectedWeapons)
    {
        if (_targetComponent.CompHP > 0)
        {
            int numWeaponsToKillShields = Mathf.CeilToInt(_targetShip.ShieldStrength / weapon.ShieldDamage);
#if FULL_DEBUG
            Debug.LogWarning("Weapon activation calculation: ");
            Debug.Log("Target shield: " + _targetShip.ShieldStrength + " weapon shield dmg " + weapon.ShieldDamage + " num to kill shield " + numWeaponsToKillShields);
#endif
            //List<ShipComponent> selectedComponents = components.Where(c => c.CompType == ComponentType.Weapon && c.gameObject.activeSelf).ToList();

            if (numWeaponsToKillShields > numSelectedWeapons)
            {
                return numSelectedWeapons;
            }
            int numWpnsToKillComp = Mathf.CeilToInt(_targetComponent.CompHP / weapon.ComponentDamage);
            int numWpnsToKillHull = Mathf.CeilToInt(_targetShip.HullHP / weapon.HullDamage);
#if FULL_DEBUG
            Debug.Log("Target comp HP: " + _targetComponent.CompHP + " weapon comp dmg " + weapon.ComponentDamage + " num to kill comp " + numWpnsToKillComp);
            Debug.Log("Target hull HP: " + _targetShip.HullHP + " weapon hull dmg " + weapon.HullDamage + " num to kill hull " + numWpnsToKillHull);
#endif
            //num weapon activations is the minimum to kill target component or to kill hull
            int totalWpnActivations = numWeaponsToKillShields + (numWpnsToKillComp < numWpnsToKillHull ? numWpnsToKillComp : numWpnsToKillHull);
            if (totalWpnActivations > numSelectedWeapons)
            {
                return numSelectedWeapons;
            }
            return totalWpnActivations;
        }
        else
        {
            return 0;
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
