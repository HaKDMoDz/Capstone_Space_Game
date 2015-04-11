using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum PlacementType { FORWARD, AFT, PORT, STARBOARD, COUNT }

public class AI_Ship : TurnBasedUnit, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Fields

    private bool receivedMoveCommand;
    private bool receivedAttackCommand;

    private float frontalAssaultRange = 50.0f;
    private Component_Weapon laser;
    public float laserRange;
    private Component_Weapon missile;
    public float missileRange;
    private Component_Weapon railgun;
    public float railgunRange;

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
    ShipComponent targetComponent = null;

    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, AI_Attack ai_Attack)
    {
        base.Init(shipBP, shipMove);
        this.ai_Attack = ai_Attack;

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

        if (TurnBasedCombatSystem.Instance.playerShips.Count > 0)
        {
            targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
            TargetComponent(targetPlayer, out targetComponent);
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
                targetComponent = GetFirstComponentInDirection(targetComponent);
                Attack();
                if (receivedAttackCommand)
                {
                    activeComponents = components;

                    trans.LookAt(targetPlayer.transform);
                    while (targetPlayer.HullHP > 0 
                        && CurrentPower > activeComponents
                        .Where(c => c.CompType == ComponentType.Weapon)
                        .Aggregate((curr, next) => curr.ActivationCost < next.ActivationCost ? curr : next).ActivationCost)
                    {
                        Debug.LogWarning(targetPlayer.HullHP);
                        yield return StartCoroutine(ActivateWeapons(targetPlayer));
                        yield return new WaitForSeconds(0.2f);
                        while (targetPlayer.GettingDestroyed)
                        {
                            Debug.LogWarning("Waiting for " + targetPlayer + " to get destroyed");
                            yield return null;
                        }

                        if (targetPlayer.ShieldStrength <= 0.0f)
                        {
                            break;
                        }
                    }
                    receivedAttackCommand = false;
#if FULL_DEBUG
                    Debug.Log(name + "- Attack end");
#endif
                }
            }
        }
        PostTurnActions();
    }

    public void RetargetNewComponent()
    {
        activeComponents = components;
        if (targetPlayer != null)
        {
            TargetComponent(targetPlayer, out targetComponent);
        }
    }

    public void RetargetNewShip()
    {
        targetPlayer = TargetEnemy(TurnBasedCombatSystem.Instance.playerShips);
    }

    protected override void PostTurnActions()
    {
        if (targetPlayer)
        {
            targetPlayer.ShowHPBars(false);
        }
    }

    public void Move(PlayerShip targetPlayer, PlacementType _placement)
    {
        if (!receivedMoveCommand)
        {
#if FULL_DEBUG
            Debug.Log("Move command received " + ShipBPMetaData.BlueprintName);
#endif
            Vector3 enemyPosition = targetPlayer.transform.position;
            Vector3 desiredPos;
            switch (_placement)
            {
                case PlacementType.FORWARD:

                    desiredPos = enemyPosition + (targetPlayer.transform.forward * (range - 10));
                    while (!CanUnitMoveTo(desiredPos))
                    {
                        Vector2 direction2D = Random.insideUnitCircle;
                        Vector3 direction3D = new Vector3(direction2D.x, desiredPos.y, direction2D.y);
                        direction3D *= (range - 10);
                        desiredPos = enemyPosition + direction3D;
                    }

                    shipMove.destination = desiredPos;

                    break;
                case PlacementType.AFT:
                    desiredPos = enemyPosition + (-targetPlayer.transform.forward * (range - 10));
                    while (!CanUnitMoveTo(desiredPos))
                    {
                        Vector2 direction2D = Random.insideUnitCircle;
                        Vector3 direction3D = new Vector3(direction2D.x, desiredPos.y, direction2D.y);
                        direction3D *= (range - 10);
                        desiredPos = enemyPosition + direction3D;
                    }

                    shipMove.destination = desiredPos;
                    break;
                case PlacementType.PORT:
                    desiredPos = enemyPosition + (-targetPlayer.transform.right * (range - 10));
                    while (!CanUnitMoveTo(desiredPos))
                    {
                        Vector2 direction2D = Random.insideUnitCircle;
                        Vector3 direction3D = new Vector3(direction2D.x, desiredPos.y, direction2D.y);
                        direction3D *= (range - 10);
                        desiredPos = enemyPosition + direction3D;
                    }
                    break;
                case PlacementType.STARBOARD:
                    desiredPos = enemyPosition + (targetPlayer.transform.right * (range - 10));
                    while (!CanUnitMoveTo(desiredPos))
                    {
                        Vector2 direction2D = Random.insideUnitCircle;
                        Vector3 direction3D = new Vector3(direction2D.x, desiredPos.y, direction2D.y);
                        direction3D *= (range - 10);
                        desiredPos = enemyPosition + direction3D;
                    }
                    break;
                case PlacementType.COUNT:
                default:
                    break;
            }
            receivedMoveCommand = true;

            float moveDistance = Vector3.Distance(shipMove.destination, trans.position);
            float movePowerCost = Mathf.Round(moveDistance * MoveCost);

            if (CurrentPower - movePowerCost >= 0)
            {
                CurrentPower -= movePowerCost;
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
    }

    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Vector3 componentGridPos = trans.position + ComponentGridTrans.position;
        Vector3 targetCompPos = component.transform.position;
        Vector3 directionToTargetComp = targetCompPos - componentGridPos;
        //Ray ray = new Ray(componentGridPos, targetCompPos - componentGridPos);
        RaycastHit[] hits = Physics.RaycastAll(componentGridPos, directionToTargetComp, GlobalVars.RayCastRange);
#if FULL_DEBUG
        if (hits == null || hits.Length == 0) Debug.LogWarning("No raycast hits");
#endif
        List<ShipComponent> hitComponents = new List<ShipComponent>();
        foreach (RaycastHit hit in hits)
        {
            ShipComponent comp = hit.collider.GetComponent<ShipComponent>();
            if (comp && !(comp.ParentShip is AI_Ship))
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
        if (components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("laser Range");
            range = laserRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("Missile range");
            range = missileRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("Railgun range");
            range = railgunRange;
        }
        else
        {
            Debug.Log("frontal Assault Range");
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
            Debug.LogWarning("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        if (idealTargetComponent == null)
        {
            TargetComponent(targetPlayer, out idealTargetComponent);
        }
        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);

    }


    private void targetCompConfLevel2(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        //lookat target
        trans.LookAt(_ship.transform);

        //select range based on longest range of component
        if (components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("laser Range");
            range = laserRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MISSILE && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("Missile range");
            range = missileRange;
        }
        else if (components.Where(c => c.CompSpecificType == ComponentSpecificType.MASS_D && c.gameObject.activeSelf).Count() > 0)
        {
            Debug.LogWarning("Railgun range");
            range = railgunRange;
        }
        else
        {
            Debug.Log("frontal Assault Range");
            range = frontalAssaultRange;
        }

        ShipComponent idealTargetComponent = null;
        //go in order of: weapons, defensive, support, engineering
        if (_ship.Components.Where(c => c.gameObject.activeSelf && c.CompType == ComponentType.Weapon).ToList().Count > 0)
        {
            Debug.LogWarning("more than 1 weapon");
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
            Debug.LogWarning("AI_Ship: Target Component Lvl 2: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);
        Debug.LogWarning("Level 2: " + _targetComponent);

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
            Debug.LogWarning("AI_Ship: Target Component Lvl 1: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);
    }

    private void targetCompConfLevel4(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        range = frontalAssaultRange;

        //make lists of components by placement
        List<ShipComponent> fwdComponents = new List<ShipComponent>();
        fwdComponents = _ship.Components.Where(c => c.Placement == PlacementType.FORWARD && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> aftComponents = new List<ShipComponent>();
        aftComponents = _ship.Components.Where(c => c.Placement == PlacementType.AFT && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> portComponents = new List<ShipComponent>();
        portComponents = _ship.Components.Where(c => c.Placement == PlacementType.PORT && c.gameObject.activeSelf).ToList<ShipComponent>();
        List<ShipComponent> starComponents = new List<ShipComponent>();
        starComponents = _ship.Components.Where(c => c.Placement == PlacementType.STARBOARD && c.gameObject.activeSelf).ToList<ShipComponent>();

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
                Debug.LogWarning("AI_Ship: Target Component Lvl 4: fwd:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
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
                Debug.LogWarning("AI_Ship: Target Component Lvl 4: aft:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
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
                Debug.LogWarning("AI_Ship: Target Component Lvl 4: port:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
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
                Debug.LogWarning("AI_Ship: Target Component Lvl 4: starboard:  NO COMPONENTS ON ENEMY SHIP FWD. assigning null target");
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
            Debug.LogWarning("AI_Ship: Target Component Lvl 5: NO COMPONENTS ON ENEMY SHIP. assigning null target");
        }

        Debug.LogWarning("Level 5 idealComp: " + idealTargetComponent);

        _targetComponent = GetFirstComponentInDirection(idealTargetComponent);

        Debug.LogWarning("Level 5 Comp: " + _targetComponent);
    }

    private float trialShipVsShip(PlayerShip _targetShip)
    {
        // calculate ai and player excess power
        float aiExcessPower = CurrentPower;
        float playerExcessPower = _targetShip.CurrentPower;

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
        //ShipComponent[] aiVsShieldComponents = Components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER).ToArray();
        float aiVsShield = 0;
        foreach (Component_Weapon component in aiVsHullComponents)
        {
            aiVsShield += component.ShieldDamage;
        }
        //ShipComponent[] playerVsShieldComponents = _targetShip.Components.Where(c => c.CompSpecificType == ComponentSpecificType.LASER).ToArray();
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
            runningAdjustment -= 0.2f;//replace with proper external weighting modifiable by GA
        }

        if (playerHP <= 0)
        {
            runningAdjustment += 0.2f;//replace with proper external weighting modifiable by GA
        }

        //return the confidence adjustment
        return runningAdjustment;
    }

    private void TargetComponent(PlayerShip _ship, out ShipComponent _targetComponent)
    {
        Debug.LogWarning("targeting a component");
        _targetComponent = null;
        if (_ship == null)
        {
            Debug.LogWarning("AIShip::_targetComponent: no ship to target");
        }
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

        if (_ship.ShieldStrength <= 0.0f)
        {
            confidence += 0.25f;
        }


        Debug.LogWarning("confidence: " + confidence);

        //setup a 5 step system ranging from kite at max range to target most valuable components first and get to them any way you can
        //set confidence to choose from those 5 options

        float confLevel1Cutoff = 0.3f;
        float confLevel2Cutoff = 0.5f;
        float confLevel3Cutoff = 0.75f;
        float confLevel4Cutoff = 0.9f;

        if (confidence < confLevel1Cutoff)
        {
            Debug.LogWarning("level one response");
            targetCompConfLevel1(_ship, out _targetComponent);
        }
        else if (confidence < confLevel2Cutoff)
        {
            Debug.LogWarning("level two response");
            targetCompConfLevel2(_ship, out _targetComponent);
            Debug.LogWarning("Level 2: " + _targetComponent);
        }
        else if (confidence < confLevel3Cutoff)
        {
            Debug.LogWarning("level three response");
            targetCompConfLevel3(_ship, out _targetComponent);
        }
        else if (confidence < confLevel4Cutoff)
        {
            Debug.LogWarning("level four response");
            targetCompConfLevel4(_ship, out _targetComponent);
        }
        else
        {
            Debug.LogWarning("level five response");
            targetCompConfLevel5(_ship, out _targetComponent);
        }

        targetComponent = GetFirstComponentInDirection(targetComponent);

        if (_targetComponent == null)
        {
            Debug.LogWarning("all component lists empty. AI cannot target components on an empty or dead ship. skipping turn");
        }

        Debug.LogWarning("Targeted Comp: " + targetComponent);
    }

    private IEnumerator ActivateWeapons(PlayerShip _targetShip)
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

        IEnumerable<Component_Weapon> weaponsToFire;

        if (_targetShip.ShieldStrength > 0.0f)
        {
            Debug.LogWarning("targetShip has sheilds");
            float maxShieldDmg = selectedWeapons.Max(c => c.ShieldDamage);
            weaponsToFire = selectedWeapons.Where(c => c.ShieldDamage == maxShieldDmg);
        }
        else
        {
            Debug.LogWarning("targetShip has NO sheilds");
            if (_targetShip.HullHP > (_targetShip.MaxHullHP * 0.75f))
            {
                Debug.LogWarning("targetShip has > 50% hull");
                float maxCompDmg = selectedWeapons.Max(c => c.ComponentDamage);
                weaponsToFire = selectedWeapons.Where(c => c.ComponentDamage == maxCompDmg);
            }
            else
            {
                Debug.LogWarning("targetShip has < 50% hull");
                float maxHullDmg = selectedWeapons.Max(c => c.HullDamage);
                weaponsToFire = selectedWeapons.Where(c => c.HullDamage == maxHullDmg);
            }
        }

        foreach (var weapon in weaponsToFire)
        {
            Debug.LogWarning(weapon.componentName);
        }

        //fire weapons
        int numWeaponsToFire;

        if (weaponsToFire.Count() > 0)
        {
            numWeaponsToFire = GetNumWeaponsToActivate(weaponsToFire.First(), _targetShip, targetComponent, weaponsToFire.Count());
        }
        else
        {
            numWeaponsToFire = 0;
            yield return null;
        }

        int weaponHitCounter = 0;

        foreach (var weapon in weaponsToFire)
        {
            if (weapon.gameObject.activeSelf && targetComponent.CompHP > 0.0f && _targetShip.HullHP > 0.0f && CurrentPower > 0.0f)
            {
                CurrentPower -= weapon.ActivationCost;
                StartCoroutine(weapon.Fire(targetComponent, () => { weaponHitCounter++; }));
                yield return new WaitForSeconds(Random.Range(PlayerShipConfig.WeaponActivationInterval.x, PlayerShipConfig.WeaponActivationInterval.y));
            }
        }

        if (targetComponent.CompHP <= 0 && _targetShip.HullHP > 0.0f)
        {
            RetargetNewComponent();
            yield return null;
        }

        while (weaponHitCounter < numWeaponsToFire)
        {
            break;
        }
    }

    /// <summary>
    /// Returns the number of weapon activations required to kill the target component or ship - avoids over-firing
    /// </summary>
    /// <returns></returns>
    private int GetNumWeaponsToActivate(Component_Weapon weapon, PlayerShip _targetShip, ShipComponent targetComponent, int numSelectedWeapons)
    {
        if (targetComponent.CompHP > 0)
        {
            int numWeaponsToKillShields = Mathf.CeilToInt(_targetShip.ShieldStrength / weapon.ShieldDamage);
#if FULL_DEBUG
            Debug.LogWarning("Weapon activation calculation: ");
            Debug.Log("Target shield: " + _targetShip.ShieldStrength + " weapon shield dmg " + weapon.ShieldDamage + " num to kill shield " + numWeaponsToKillShields);
#endif
            if (numWeaponsToKillShields > numSelectedWeapons)
            {
                return numSelectedWeapons;
            }
            int numWpnsToKillComp = Mathf.CeilToInt(targetComponent.CompHP / weapon.ComponentDamage);
            int numWpnsToKillHull = Mathf.CeilToInt(_targetShip.HullHP / weapon.HullDamage);
#if FULL_DEBUG
            Debug.LogWarning("Target comp HP: " + targetComponent.CompHP + " weapon comp dmg " + weapon.ComponentDamage + " num to kill comp " + numWpnsToKillComp);
            Debug.LogWarning("Target hull HP: " + _targetShip.HullHP + " weapon hull dmg " + weapon.HullDamage + " num to kill hull " + numWpnsToKillHull);
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
