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

    private PlayerShip targetShip;

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

    private void targetCompConfLevel1(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);

        //setup raytrace
        float distanceToTarget;
        RaycastHit hit;

        Debug.DrawRay(trans.position + ComponentGridTrans.position, ((_ship.transform.position + _ship.ComponentGridTrans.position) - (trans.position + ComponentGridTrans.position)), Color.red, 1000.0f, false);
        if (Physics.Raycast(transform.position + ComponentGridTrans.position, ((_ship.transform.position + _ship.ComponentGridTrans.position) - (trans.position + ComponentGridTrans.position)), out hit, 1 << TagsAndLayers.ComponentsLayer))
        {
            distanceToTarget = hit.distance;
            Debug.LogError(hit.collider.name + " : " + distanceToTarget);
        }
        _targetComponent = hit.collider.GetComponent<ShipComponent>();
        Debug.LogError(_targetComponent + ":" + _targetComponent.Placement);

        //go in order of: weapons, defensive, support, engineering
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

    private void targetCompConfLevel2(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 2: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private void targetCompConfLevel3(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //make lists of components by placement
        List<ShipComponent> fwdComponents = new List<ShipComponent>();
        fwdComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.FORWARD && c.CanActivate).ToList<ShipComponent>();
        List<ShipComponent> aftComponents = new List<ShipComponent>();
        aftComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.AFT && c.CanActivate).ToList<ShipComponent>();
        List<ShipComponent> portComponents = new List<ShipComponent>();
        portComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.PORT && c.CanActivate).ToList<ShipComponent>();
        List<ShipComponent> starComponents = new List<ShipComponent>();
        starComponents = _ship.Components.Where(c => c.Placement == AI_Fleet.PlacementType.STARBOARD && c.CanActivate).ToList<ShipComponent>();

        //determine list with least defense
        int fwdArmour, aftArmour, portArmour, starArmour;

        fwdArmour = fwdComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        aftArmour = aftComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        portArmour = portComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        starArmour = starComponents.Where(c => c.CompType == ComponentType.Defense).ToList<ShipComponent>().Count;
        
        //target from that side
        if (fwdArmour <= aftArmour && fwdArmour <= portArmour && fwdArmour <= starArmour)
        {
            //fwd is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (fwdComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = fwdComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 3: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else if (aftArmour <= fwdArmour && aftArmour <= portArmour && aftArmour <= starArmour)
        {
            //aft is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (aftComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = aftComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 3: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else if (portArmour <= fwdArmour && portArmour <= aftArmour && portArmour <= starArmour)
        {
            //port is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (portComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = portComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 3: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }
        else
        {
            //starboard is lowest armoured front
            //go in order of: defensive, engineering, weapon, support
            if (starComponents.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else if (starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).ToList().Count > 0)
            {
                _targetComponent = starComponents.Where(c => c.CanActivate && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
            }
            else
            {
                _targetComponent = null;
                Debug.LogError("AI_Ship: Target Component Lvl 3: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
            }
        }

        
        
    }

    private void targetCompConfLevel4(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        _targetComponent = null;
        Debug.LogError("AI_Ship: Target Component Lvl 4: NO COMPONENTS ON ENEMY SHIP. assigning null target");
    }

    private void targetCompConfLevel5(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //go in order of: engineering, support, weapons, defensive
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Engineering).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Support).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).ToList().Count > 0)
        {
            _targetComponent = _ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Defense).Aggregate((curr, next) => curr.CompHP <= next.CompHP ? curr : next);
        }
        else
        {
            _targetComponent = null;
            Debug.LogError("AI_Ship: Target Component Lvl 5: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }
        
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

        while (aiHP >= 0 && playerHP >= 0)
        {
            //run a typical turn

            //simple version. more adv. version to come later
            //take damage from each other
            Debug.Log(CurrentPower);
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

            //damage the ai
            if (aiMaxShield >= 0)
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
        }

        Debug.LogError("AI: " + aiHP);
        Debug.LogError("player: " + playerHP);
        if (aiHP <= 0)
        {
            Debug.Log("AI ship died during mock encounter");
            runningAdjustment -= 0.25f;//replace with proper external weighting modifiable by GA
        }

        if (playerHP <= 0)
        {
            Debug.Log("player ship died during mock encounter");
            runningAdjustment += 0.25f;//replace with proper external weighting modifiable by GA
        }

        //return the confidence adjustment
        Debug.LogWarning("total confidence adjuster: " + runningAdjustment);
        return runningAdjustment;
    }

    private ShipComponent TargetComponent(PlayerShip _ship)
    {
        ShipComponent _targetComponent = null ;

        //float confidence = Random.Range(0.0f, 1.0f);
        float confidence = 0;


        //get count for player fleet
        int playerFleetStrength = GameController.Instance.GameData.playerFleetData.gridIndex_metaData_table.Count();
        // get count for enemy fleet
        int aiFleetStrength = GameController.Instance.GameData.pirates_AI_Data.currentFleet_BlueprintNames.Count();

        //calculate basis for confidence

        int totalShips = playerFleetStrength + aiFleetStrength;

        confidence = (float)aiFleetStrength / (float)totalShips;

        //adjust based on ship and target

        confidence += trialShipVsShip(_ship);

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
