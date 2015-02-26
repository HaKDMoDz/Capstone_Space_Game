#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
#endregion Usings

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    private CombatSystemInterface combatInterface;
    public PlayerAttack playerAttack { get; private set; }
    private SpaceGround spaceGround;
    private LineRenderer line;
    private Material lineMat;
    private Color validColour = Color.cyan;
    private Color invalidColour = Color.red;

    //helper
    private bool continueTurn = true;
    private bool registerInput;
    private bool receivedMoveCommand;
    private bool componentSelectionOn;
    private bool attackTargetConfirmed = false;
    private bool targetNext = false;
    private bool startTargetingSequence;
    private bool dragging;
    private bool takingTurn = false;
    private bool firing = false;
    private float totalActivationCost = 0.0f;
    private Vector3 mousePosOnGround = Vector3.zero;
    private float moveDistance = 0.0f;
    private float movePowerCost = 0.0f;
    
    
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();
    private ShipComponent targetComponent = null;
    private AI_Ship targetShip = null;

    #endregion Fields   

    #region Methods
    #region PublicMethods
    /// <summary>
    /// initializes the various components of the ship and setups up references
    /// </summary>
    /// <param name="shipBP"></param>
    /// <param name="shipMove"></param>
    /// <param name="playerAttack"></param>
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, PlayerAttack playerAttack)
    {
        base.Init(shipBP, shipMove);
        this.playerAttack = playerAttack;
        this.playerAttack.Init();
        combatInterface = CombatSystemInterface.Instance;
        ConfigurePlayerShip();
        
        //InputManager.Instance.RegisterKeysDown(
        //    (key) => { componentSelectionOn = false; },
        //    KeyCode.Space);

        //InputManager.Instance.RegisterKeysDown(
        //    (key) => { continueTurn = false; },
        //   KeyCode.KeypadEnter, KeyCode.Return
        //    );

        InputManager.Instance.OnMouseMoveEvent += OnMouseMove;
        spaceGround = SpaceGround.Instance;
        foreach (ShipComponent component in components)
        {
            component.OnComponentClicked += OnComponentClicked;
        }
        spaceGround.OnGroundClick += SpaceGroundClick;
    }
    protected override void PreTurnActions()
    {
        continueTurn = true;
        totalActivationCost = 0.0f;
        SubscribeToAIShipMouseEvents(true);
        //setups up the GUI for the player ship
        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowStatsPanel(true);
        //combatInterface.ShowComponentActivationButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        combatInterface.ShowPower(CurrentPower);
    }
    /// <summary>
    /// Starts the turn for the player ship. Starts listening for commands to move or to activate components
    /// </summary>
    /// <returns></returns>
    public override IEnumerator ExecuteTurn()
    {
        takingTurn = true;
        
        yield return StartCoroutine(base.ExecuteTurn());

        #if FULL_DEBUG
        Debug.Log("Player unit turn");
        #endif

        PreTurnActions();

        //run until the the player hits Return or until power reaches 0
        while (continueTurn)
        {
            //Show movement UI
            ShowMovementUI(true);
            GetMouseOverPosOnSpaceGround();

            //move the ship - does not use power right now
            if (receivedMoveCommand)
            {
                UnSelectComponents(false);
                ShowMovementUI(false);
                CurrentPower -= movePowerCost;
                yield return StartCoroutine(shipMove.Move());
                #if FULL_DEBUG
                Debug.Log(ShipBPMetaData.BlueprintName + "- Movement end");
                #endif
            }
            if(startTargetingSequence)
            {
                ShowMovementUI(false);
                yield return StartCoroutine(ComponentSelectionAndTargeting());
            }

            //if the player has started selecting components
            //if (componentSelectionOn)
            //{
            //    ShowMovementUI(false);
            //    //lets the player select whatever components he/she wants
            //    yield return StartCoroutine(ComponentSelectionSequence());
            //    combatInterface.ShowComponentSelectionPanel(false);
            //    Debug.Log("activating components");
            //    firing = true;
            //    //activates the components with a callback when the activation is complete with the power used by components successfully activated
            //    yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents, 
            //        (float activationCost)=> 
            //            {
            //                CurrentPower -= activationCost;
            //            }));
            //    firing = false;
            //    UnSelectComponents(false);
            //}
            receivedMoveCommand = false;
            if(CurrentPower <=0) //end turn
            {
                continueTurn = false;
            }
            yield return null;
        }
        PostTurnActions();

    }//ExecuteTurn

    protected override void PostTurnActions()
    {
        takingTurn = false;
        SubscribeToAIShipMouseEvents(false);
        //de-activate GUI
        ShowMovementUI(false);
        combatInterface.ShowComponentActivationButtons(null, null);
        combatInterface.ShowStatsPanel(false);
    }

    /// <summary>
    /// Called by the gui interface to select all components of a given type
    /// </summary>
    /// <param name="compType"></param>
    public void SelectAllComponents(Type compType)
    {
        if(firing)
        {
            return;
        }
        Debug.Log("Activating all " + compType.ToString());
        UnSelectComponents(true);
        componentSelectionOn = true;
        foreach (ShipComponent component in components.Where(c=>c.GetType()==compType))
        {
            SelectComponent(component, true);
        }
    }
    public void TargetNext(KeyCode key)
    {
        targetNext = true;
    }
    public void StopTargetingSequence(KeyCode key)
    {
        attackTargetConfirmed = true;
    }
    #endregion PublicMethods


    #region PrivateMethods

    /// <summary>
    /// Show a line renderer going to the mouse position and set's the colour based whether the ship has enough power. Also shows asks the combat Interface to show the Movement GUI
    /// </summary>
    /// <param name="show"></param>
    private void ShowMovementUI(bool show)
    {
        if(show)
        {
            Color lineColour = validColour;
            if(movePowerCost > CurrentPower)
            {
                lineColour = invalidColour;
            }
            DisplayLineRenderer(mousePosOnGround, true, lineColour);
            combatInterface.ShowMoveCostUI(mousePosOnGround, moveDistance, moveDistance * MoveCost);
        }
        else
        {
            combatInterface.HideMoveUI();
            DisplayLineRenderer(mousePosOnGround, false, validColour);
        }

        //TODO: check collision
    }
    
    /// <summary>
    /// Start the component selection sequence - let's the player click on components in the component selection panel
    /// </summary>
    /// <returns></returns>
    //private IEnumerator ComponentSelectionSequence()
    //{
    //    Debug.Log("Selecting Components - [Space] to confirm");
    //    while (componentSelectionOn)
    //    {
    //        yield return null;
    //    }
    //    componentSelectionOn = false;
    //}
    private IEnumerator ComponentSelectionAndTargeting()
    {
        //show comp seleciton panel
        combatInterface.ShowComponentSelectionPanel(true);
        //show hotkeys
        combatInterface.ShowComponentActivationButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        //show enemy target
        int targetShipIndex = 0;
        List<AI_Ship> aiShips = TurnBasedCombatSystem.Instance.ai_Ships;
        int numAIShips = aiShips.Count;
        #if !NO_DEBUG
        if (numAIShips == 0)
        {
            Debug.LogError("No ai ships found");
        }
        #endif
        targetShip = aiShips[targetShipIndex];
        Transform aiTargetTrans = targetShip.transform;
        //camera
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTargetTrans, GlobalVars.CameraAimAtPeriod));
        trans.LookAt(aiTargetTrans);
        ShowTargetingPanel(true);
        InputManager.Instance.RegisterKeysDown(TargetNext, KeyCode.Tab);
        InputManager.Instance.RegisterKeysDown(StopTargetingSequence, KeyCode.Escape);

        while(!attackTargetConfirmed)
        {
            if(targetNext)
            {
                ShowTargetingPanel(false);
                targetShipIndex = ++targetShipIndex % numAIShips;
                targetShip = aiShips[targetShipIndex];
                ShowTargetingPanel(true);
                aiTargetTrans = targetShip.transform;
                yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTargetTrans, GlobalVars.CameraAimAtPeriod));
                trans.LookAt(aiTargetTrans);
                targetNext = false;
            }

            yield return null;
        }
        ShowTargetingPanel(false);
        InputManager.Instance.DeregisterKeysDown(TargetNext, KeyCode.Tab);
        InputManager.Instance.DeregisterKeysDown(StopTargetingSequence, KeyCode.Escape);
        //listen for mouse on enemy comps
        //listen for mouse on self comps
        //if not any selected comps, invalidate enemy targeting
        //once selected, allow enemy comp targeting
        //once clicked on ememy comp
            //activation sequence
        yield return null;
    }
    private void ShowTargetingPanel(bool show)
    {
        if(!targetShip)
        {
            return;
        }
        if(show)
        {
            targetShip.ShowTargetingPanel(true, trans);
        }
        else
        {
            DisplayLineRenderer(Vector3.zero, false, validColour);//hide line
            targetShip.ShowTargetingPanel(false, null);
        }
    }
    private void SubscribeTargetShipComponentEvents(bool subscribe)
    {
#if FULL_DEBUG
        if(targetShip==null)
        {
            Debug.LogError("Target ship is null");
            return;
        }
#endif
        if (subscribe)
        {
            foreach (ShipComponent component in targetShip.Components)
            {
                component.OnComponentClicked += OnTargetShipComponentClicked;
                component.OnComponentMouseOver += OnTargetShipComponentMouseOver;
                component.OnComponentPointerExit += OnTargetShipComponentPointerExit;
            }
        }
        else
        {
            foreach (ShipComponent component in targetShip.Components)
            {
                component.OnComponentClicked -= OnTargetShipComponentClicked;
                component.OnComponentMouseOver -= OnTargetShipComponentMouseOver;
                component.OnComponentPointerExit -= OnTargetShipComponentPointerExit;
            }
        }
    }
    void OnTargetShipComponentClicked(ShipComponent component)
    {
        #if FULL_DEBUG
        if(component==null)
		{
            Debug.LogError("point click component null")  ;
        }
        #endif
        if(targetComponent)
        {
            targetComponent.Selected = false;
        }
        targetComponent = GetFirstComponentInDirection(component);
        targetComponent.Selected = true;
        attackTargetConfirmed = true;
    }
    void OnTargetShipComponentMouseOver(ShipComponent component)
    {
        #if FULL_DEBUG
        if(component==null)
		{
            Debug.LogError("point over component null")  ;
        }
        #endif
        if (targetComponent)
        {
            targetComponent.Selected = false;
        }
        targetComponent = GetFirstComponentInDirection(component);
        Debug.Log("TargetComp: " + targetComponent);

        DisplayLineRenderer(targetComponent.transform.position, true, validColour);
        targetComponent.Selected = true;
    }
    void OnTargetShipComponentPointerExit(ShipComponent component)
    {
        #if FULL_DEBUG
        if(component==null)
		{
            Debug.LogError("point exit component null")  ;
        }
	    #endif
        if (targetComponent)
        {
            targetComponent.Selected = false;
        }
        if (component.Selected)
        {
            component.Selected = false;
        }
    }
    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Ray ray = new Ray(trans.position, component.transform.position - trans.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1 << TagsAndLayers.ComponentsLayer))
        {
            return hit.collider.GetComponent<ShipComponent>();
        }
        return component;
    }
    private void AllowEnemyTargeting(bool allow)
    {
        SubscribeTargetShipComponentEvents(allow);
    }
    private void UnSelectComponents(bool resetPower)
    {
        foreach (ShipComponent component in components)
        {
            SelectComponent(component, false);
        }
    }
    /// <summary>
    /// Select the specified component - shows the effect and updates GUI to reflect potential power cost
    /// </summary>
    /// <param name="component"></param>
    /// <param name="select"></param>
    private void SelectComponent(ShipComponent component, bool select)
    {
        if (select) 
        {
            if (!selectedComponents.Contains(component))
            {
                //Debug.Log("select");
                componentSelectionOn = true;
                if(CurrentPower - totalActivationCost - component.ActivationCost < 0)
                {
                    Debug.LogWarning("Not enough power");
                    return;
                }
                selectedComponents.Add(component);
                component.Selected = true;
                AllowEnemyTargeting(true);
                totalActivationCost += component.ActivationCost;
                combatInterface.ShowPower(CurrentPower - totalActivationCost);
            }
        }
        else //de-select
        {
            if(selectedComponents.Contains(component))
            {
                selectedComponents.Remove(component);
                component.Selected = false;
                if(selectedComponents.Count==0)
                {
                    AllowEnemyTargeting(false);
                }
                totalActivationCost -= component.ActivationCost;
                combatInterface.ShowPower(CurrentPower - totalActivationCost);
            }
        }
    }//SelectComponent
    private void SubscribeToAIShipMouseEvents(bool subscribe)
    {
        if(subscribe)
        {
            foreach (AI_Ship aiShip in TurnBasedCombatSystem.Instance.ai_Ships)
            {
                aiShip.OnShipClick += aiShip_OnShipClick;
                aiShip.OnShipMouseEnter += aiShip_OnShipMouseEnter;
                aiShip.OnShipMouseExit += aiShip_OnShipMouseExit;
            }
        }
        else
        {
            foreach (AI_Ship aiShip in TurnBasedCombatSystem.Instance.ai_Ships)
            {
                aiShip.OnShipClick -= aiShip_OnShipClick;
                aiShip.OnShipMouseEnter -= aiShip_OnShipMouseEnter;
                aiShip.OnShipMouseExit -= aiShip_OnShipMouseExit;
            }
        }
    }

    void aiShip_OnShipMouseExit(AI_Ship ship)
    {
        combatInterface.ShowAttackCursor(false);
    }
    void aiShip_OnShipMouseEnter(AI_Ship ship)
    {
        if (!startTargetingSequence)
        {
            combatInterface.ShowAttackCursor(true);
        }
    }
    void aiShip_OnShipClick(AI_Ship ship)
    {
        combatInterface.ShowAttackCursor(false);
        startTargetingSequence = true;
    }

    #region InternalCallbacks
    /// <summary>
    /// clicked on ground - move command
    /// </summary>
    /// <param name="worldPosition"></param>
    void SpaceGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("Click on ground at position: "+worldPosition);
        if (takingTurn && !receivedMoveCommand && movePowerCost <= CurrentPower)
        {
            Debug.Log("Move command received " + ShipBPMetaData.BlueprintName);
            shipMove.destination = worldPosition;
            receivedMoveCommand = true;
        }
    }
    /// <summary>
    /// clicked on a component - toggle selection, start component selection sequnce if required
    /// </summary>
    /// <param name="component"></param>
    void OnComponentClicked(ShipComponent component)
    {
        SelectComponent(component, !selectedComponents.Contains(component));
    }

    /// <summary>
    /// Called whenever the mouse is moved
    /// </summary>
    /// <param name="direction"></param>
    void OnMouseMove(Vector2 direction)
    {
        GetMouseOverPosOnSpaceGround();
    }
    #endregion InternalCallbacks

    //Helper

    /// <summary>
    /// Gets the lineRenderer prefab from Resources, instantiates and assigns to the playership
    /// </summary>
    private void ConfigurePlayerShip()
    {
        //add line renderer for movement and targeting
        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/LineRenderer");

        GameObject lineObj = Instantiate(linePrefab) as GameObject;
        lineObj.transform.SetParent(transform, false);
        line = lineObj.GetComponent<LineRenderer>();
        lineMat = line.renderer.material;
        playerAttack.line = line;
        line.enabled = false;
    }

    /// <summary>
    /// Displays the line renderer going to the target position and set's the colour.
    /// </summary>
    /// <param name="targetPos"> the end point for the line renderer</param>
    /// <param name="show">Whether to show the line renderer or not</param>
    /// <param name="color">The colour for the line </param>
    private void DisplayLineRenderer(Vector3 targetPos, bool show , Color color)
    {
        lineMat.SetColor("_TintColor", color);
        line.enabled = show;
        if (!show)
        {
            return;
        }
        line.SetVertexCount(2);
        line.SetPosition(0, trans.position);
        line.SetPosition(1, targetPos);
    }
    /// <summary>
    /// Gets the world position on the space ground from the mouse position
    /// </summary>
    private void GetMouseOverPosOnSpaceGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1<<TagsAndLayers.SpaceGroundLayer))
        {
            mousePosOnGround = hit.point;
            moveDistance = Vector3.Distance(mousePosOnGround, trans.position);
            movePowerCost = Mathf.Round(moveDistance * MoveCost);
            
        }
    }
    #endregion PrivateMethods
    #endregion Methods
}
