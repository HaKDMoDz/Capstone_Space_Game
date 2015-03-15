/*
  PlayerShip.cs
  Mission: Invasion
  Created by Rohun Banerji on Jan 13/2015
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

#region Usings
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
#endregion Usings

public class PlayerShip_Old : TurnBasedUnit
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
    //private bool componentSelectionOn;
    private bool attackTargetConfirmed = false;
    private bool targetNext = false;
    private bool startTargetingSequence;
    private bool stopTargetingSequence=false;
    //private bool dragging;
    private bool takingTurn = false;
    private bool firing = false;
    private bool allowingEnemyTargeting = false;
    private float totalActivationCost = 0.0f;
    private Vector3 mousePosOnGround = Vector3.zero;
    private float moveDistance = 0.0f;
    private float movePowerCost = 0.0f;
    
    
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();
    private ShipComponent targetComponent = null;
    private AI_Ship targetShip = null;
    private Transform targetingArcTrans;

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
        spaceGround = SpaceGround.Instance;
        ConfigurePlayerShip();
        
    }
    private void EnableInputEvents(bool enable)
    {
        if(enable)
        {
            InputManager.Instance.OnMouseMoveEvent += OnMouseMove;
            InputManager.Instance.RegisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
            foreach (ShipComponent component in components.Where(comp=>comp.CanActivate))
            {
                component.OnComponentClicked += OnComponentClicked;
            }
            spaceGround.OnGroundClick += SpaceGroundClick;
        }
        else
        {
            InputManager.Instance.OnMouseMoveEvent -= OnMouseMove;
            InputManager.Instance.DeregisterKeysDown(EndTurn, KeyCode.KeypadEnter, KeyCode.Return);
            foreach (ShipComponent component in components.Where(comp => comp.CanActivate))
            {
                component.OnComponentClicked -= OnComponentClicked;
            }
            spaceGround.OnGroundClick -= SpaceGroundClick;
        }
    }
    protected override void PreTurnActions()
    {
        continueTurn = true;
        totalActivationCost = 0.0f;
        SubscribeToAIShipMouseEvents(true);
        //setups up the GUI for the player ship
        //combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowStatsPanel(true);
        //combatInterface.ShowComponentActivationButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        combatInterface.UpdateStats(CurrentPower, MoveCost);
        combatInterface.SetPowerValid();
        EnableInputEvents(true);
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
            ShowTargetingArc(true);

            //move the ship - does not use power right now
            if (receivedMoveCommand)
            {
                ShowTargetingArc(false);
                UnSelectComponents(false);
                ShowMovementUI(false);
                CurrentPower -= movePowerCost;
                TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.MovementHowTo);
                yield return StartCoroutine(shipMove.Move());
                #if FULL_DEBUG
                Debug.Log(ShipBPMetaData.BlueprintName + "- Movement end");
                #endif
            }
            if(startTargetingSequence)
            {
                ShowMovementUI(false);
                ShowTargetingArc(false);
                TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ClickEnemyToEngage, false);
                yield return StartCoroutine(ComponentSelectionAndTargeting());
                
            }
            if(targetComponent)
            {
                ShowTargetingArc(false);
                yield return StartCoroutine(ActivateWeapons());
                EnableInputEvents(true);
            }//if(targetComponent)
            receivedMoveCommand = false;
            spaceGround.Display(true);
            if(CurrentPower <=0 || TurnBasedCombatSystem.Instance.ai_Ships.Count ==0) //end turn
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
        DisplayLineRenderer(Vector3.zero, false, validColour);
        combatInterface.ShowComponentHotkeyButtons(null, null);
        combatInterface.ShowStatsPanel(false);
        EnableInputEvents(false);
    }
    private IEnumerator ComponentSelectionAndTargeting()
    {
        //stop listening to mouse events on ai ships
        SubscribeToAIShipMouseEvents(false);
        //show enemy target
        int targetShipIndex = 0;
        List<AI_Ship> aiShips = TurnBasedCombatSystem.Instance.ai_Ships;
        int numAIShips = aiShips.Count;
        #if FULL_DEBUG
        if (numAIShips == 0)
        {
            Debug.LogError("No ai ships found");
        }
        #endif

        //targetShip = aiShips[targetShipIndex];
        targetShipIndex = aiShips.IndexOf(targetShip);
        Transform aiTargetTrans = targetShip.transform;
        spaceGround.Display(false);
        //camera
        yield return StartCoroutine(CameraDirector.Instance.OverheadAimAt(trans, aiTargetTrans, GlobalVars.CameraAimAtPeriod));
        //show comp seleciton panel
        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowComponentSelectionPanel(true);
        //show hotkeys
        combatInterface.ShowComponentHotkeyButtons(SelectAllComponents, components.Where(c => c.CanActivate));
        ShowTargetingPanel(true);
        trans.LookAt(aiTargetTrans);
        InputManager.Instance.RegisterKeysDown(TargetNext, KeyCode.Tab);
        InputManager.Instance.RegisterKeysDown(StopTargetingSequence, KeyCode.Escape);
        TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.ClickEnemyToEngage);

        while (!attackTargetConfirmed)
        {
            if (stopTargetingSequence)
            {
                startTargetingSequence = false;
                stopTargetingSequence = false;
                targetComponent = null;
                ShowTargetingPanel(false);
                combatInterface.ShowComponentSelectionPanel(false);
                InputManager.Instance.DeregisterKeysDown(TargetNext, KeyCode.Tab);
                InputManager.Instance.DeregisterKeysDown(StopTargetingSequence, KeyCode.Escape);
                yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
                break;
            }
            if (targetNext)
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
        SubscribeToAIShipMouseEvents(true);
        combatInterface.ShowComponentSelectionPanel(false);
        InputManager.Instance.DeregisterKeysDown(TargetNext, KeyCode.Tab);
        InputManager.Instance.DeregisterKeysDown(StopTargetingSequence, KeyCode.Escape);
    }
    private IEnumerator ActivateWeapons()
    {
        if(TutorialSystem.Instance)
        {
            TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.ClickOnCompToFire, false);
        }
        int numWeaponsActivated = 0;
        float totalPowerUsed = 0.0f;
        int originalCamCulling = Camera.main.cullingMask;
        EnableInputEvents(false);
        DisplayLineRenderer(Vector3.zero, false, validColour);
        ShowTargetingPanel(false);
        AllowEnemyTargeting(false);
        combatInterface.ShowStatsPanel(false);
        SubscribeToAIShipMouseEvents(false);
        targetComponent.Selected = false;
        combatInterface.ShowComponentHotkeyButtons(null, null);
        //combatInterface.ShowComponentSelectionPanel(false);
        targetShip.ShowHPBars(true);
        Camera.main.cullingMask = originalCamCulling | 1 << TagsAndLayers.ComponentsLayer | 1 << TagsAndLayers.ComponentSlotLayer;
        yield return StartCoroutine(CameraDirector.Instance.ZoomInFromAbove(targetComponent.ParentShip.transform, GlobalVars.CameraAimAtPeriod));
        trans.LookAt(targetComponent.transform);
        if(TutorialSystem.Instance)
        {
            TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.EnemyShieldHP, true);
        }
        if (selectedComponents.Any(c => !(c is Component_Weapon)))
        {
            Debug.LogError("Not weapon ");
        }
        foreach (Component_Weapon weapon in selectedComponents)
        {
            if (!targetShip || targetShip.HullHP<=0.0f)
                break;
            if (targetComponent && targetComponent.CompHP > 0.0f)
            {
                yield return StartCoroutine(
                    weapon.Fire(targetComponent,
                    () =>
                    {
                        numWeaponsActivated--;
                        totalPowerUsed += weapon.ActivationCost;
                    }));
                numWeaponsActivated++;
            }
        }
        //waits until all weapons have completed their animation
        while (numWeaponsActivated > 0 && targetShip && targetShip.HullHP>0.0f)
        {
            Debug.LogWarning("targetship hp " + targetShip.HullHP);
            yield return null;
        }
        Camera.main.cullingMask = originalCamCulling;
        
        CurrentPower -= totalActivationCost;
        UnSelectComponents(false);
        combatInterface.ShowStatsPanel(true);
        attackTargetConfirmed = false;
        
        targetComponent = null;
        if (targetShip && targetShip.HullHP > 0.0f)
        {
            targetShip.ShowHPBars(false);
            SubscribeToAIShipMouseEvents(true);
        }
        else
        {
            Debug.Log("Acitvation - target dead");
            startTargetingSequence = false;
            yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
        }
        //yield return StartCoroutine(CameraDirector.Instance.MoveToFocusOn(trans, GlobalVars.CameraMoveToFocusPeriod));
    }//ActivateWeapons()
    /// <summary>
    /// Accessed by the GUI button
    /// </summary>
    public void EndTurn()
    {
        EndTurn(KeyCode.Return);
    }
    /// <summary>
    /// ends the current turn
    /// </summary>
    /// <param name="key"></param>
    public void EndTurn(KeyCode key)
    {
        if(TutorialSystem.Instance)
        {
            TutorialSystem.Instance.ShowTutorial(TutorialSystem.TutorialType.EndTurn, false);
        }
        continueTurn = false;
        if(startTargetingSequence) StopTargetingSequence(KeyCode.Return);
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
        //componentSelectionOn = true;
        TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.Hotkeys);
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
        stopTargetingSequence = true;
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
            combatInterface.HideTooltip();
        }
    }
    private void SubscribeTargetShipComponentEvents(bool subscribe)
    {
#if FULL_DEBUG
        if(!targetShip)
        {
            Debug.LogWarning("Target ship is null");
            return;
        }
        //Debug.Log("Subscribe: " + subscribe);
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
        if(!targetComponent)
        {
            Debug.LogWarning("No target comp ");
            return;
        }
        //Debug.Log("TargetComp: " + targetComponent + " from ship " + name);
        combatInterface.ShowToolTip(component.componentName, Input.mousePosition);
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
        combatInterface.HideTooltip();
    }
    private ShipComponent GetFirstComponentInDirection(ShipComponent component)
    {
        Ray ray = new Ray(trans.position + ComponentGridTrans.position, component.transform.position - (trans.position + ComponentGridTrans.position));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GlobalVars.RayCastRange, 1 << TagsAndLayers.ComponentsLayer))
        {
            return hit.collider.GetComponent<ShipComponent>();
        }
        return component;
    }
    private void AllowEnemyTargeting(bool allow)
    {
        allowingEnemyTargeting = allow;
        if (!allow) DisplayLineRenderer(Vector3.zero, false, validColour);
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
                if(selectedComponents.Count>0)
                {
                    //not the same component type as selected - restart selection
                    if(component.GetType() != selectedComponents[0].GetType())
                    {
                        UnSelectComponents(true);
                    }
                }
                if (CurrentPower - totalActivationCost - component.ActivationCost < 0)
                {
                    #if FULL_DEBUG
                    Debug.LogWarning("Not enough power");
                    #endif
                    combatInterface.SetPowerValid(false);
                    return;
                }
                selectedComponents.Add(component);
                component.Selected = true;
                TutorialSystem.Instance.ShowNextTutorial(TutorialSystem.TutorialType.ComponentSelection);
                if (!allowingEnemyTargeting) AllowEnemyTargeting(true);
                totalActivationCost += component.ActivationCost;
                combatInterface.UpdateStats(CurrentPower - totalActivationCost, MoveCost);

            }//selected component contains
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
                combatInterface.UpdateStats(CurrentPower - totalActivationCost, MoveCost);
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
    private void ShowTargetingArc(bool show)
    {
        targetingArcTrans.gameObject.SetActive(show);
    }
    void aiShip_OnShipMouseExit(AI_Ship ship)
    {
        combatInterface.ShowAttackCursor(false);
        ship.ShowHPBars(false);
    }
    void aiShip_OnShipMouseEnter(AI_Ship ship)
    {
        if (!startTargetingSequence)
        {
            combatInterface.ShowAttackCursor(true);
            ship.ShowHPBars(true);
        }
    }
    void aiShip_OnShipClick(AI_Ship ship)
    {
        combatInterface.ShowAttackCursor(false);
        startTargetingSequence = true;
        targetShip = ship;
        Debug.Log("on Ship click " + ship);
        ship.ShowHPBars(false);
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
        targetingArcTrans.LookAtWithSameY(mousePosOnGround);
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
        
        //setup targeting arcs
        GameObject targetingArcs = new GameObject("TargetingArcs");
        targetingArcTrans = targetingArcs.transform;
        targetingArcTrans.Translate(Vector3.up);
        //targetingArcTrans.RotateAroundXAxis(90.0f);
        targetingArcTrans.SetParent(trans, false);
        foreach (Type type in components.Where(c=>c is Component_Weapon).Select(c=>c.GetType()).Distinct())
        {
            Component_Weapon weapon = (Component_Weapon)components.First(c => c.GetType() == type);
            GameObject arcObj = new GameObject(weapon.componentName + " Arc");
            arcObj.transform.SetParent(targetingArcTrans, false);
            arcObj.transform.RotateAroundXAxis(90.0f);
            ArcMesh arc = arcObj.AddComponent<ArcMesh>();
            Material arcMat = new Material(PlayerShipConfig.ArcMat);
            arcMat.color = weapon.WeaponColour.WithAplha(PlayerShipConfig.ArcAlpha);
            arc.BuildArc(weapon.range, PlayerShipConfig.ArcAngle, PlayerShipConfig.ArcSegments, arcMat);
        }
        ShowTargetingArc(false);
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
        line.SetPosition(0, trans.position + ComponentGridTrans.position);
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
