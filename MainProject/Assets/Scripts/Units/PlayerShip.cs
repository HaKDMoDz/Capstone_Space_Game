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
    [SerializeField]
    private LineRenderer line;

    //helper
    private bool continueTurn = true;
    private bool registerInput;
    private bool receivedMoveCommand;
    private bool componentSelectionOn;
    private bool dragging;
    private bool takingTurn = false;
    private bool firing = false;
    private float totalActivationCost = 0.0f;
    
    
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();


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

        combatInterface = CombatSystemInterface.Instance;

        this.playerAttack = playerAttack;
        this.playerAttack.Init();

        ConfigurePlayerShip();
        
        InputManager.Instance.RegisterKeysDown(
            (key) => { componentSelectionOn = false; },
            KeyCode.Space);

        InputManager.Instance.RegisterKeysDown(
            (key) => { continueTurn = false; },
           KeyCode.KeypadEnter, KeyCode.Return
            );

        
        spaceGround = SpaceGround.Instance;
        foreach (ShipComponent component in components)
        {
            component.OnComponentClicked += OnComponentClicked;
        }
        spaceGround.OnGroundClick += SpaceGroundClick;
    }

    /// <summary>
    /// Starts the turn for the player ship. Starts listening for commands to move or to activate components
    /// </summary>
    /// <returns></returns>
    public override IEnumerator ExecuteTurn()
    {
        takingTurn = true;
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");

        continueTurn = true;
        totalActivationCost = 0.0f;

        //setups up the GUI for the player ship
        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowStatsPanel(true);
        combatInterface.ShowComponentActivationButtons(SelectAllComponents, components.Where(c=>c.CanActivate));
        combatInterface.ShowPower(CurrentPower);

        //run until the the player hits Return or until power reaches 0
        while (continueTurn)
        {
            //move the ship - does not use power right now
            if (receivedMoveCommand)
            {
                UnSelectComponents(false);
                yield return StartCoroutine(shipMove.Move());
                #if FULL_DEBUG
                Debug.Log(shipBPMetaData.BlueprintName + "- Movement end");
                #endif
            }

            //if the player has started selecting components
            if (componentSelectionOn)
            {
                //lets the player select whatever components he/she wants
                yield return StartCoroutine(ComponentSelectionSequence());
                Debug.Log("activating components");
                firing = true;
                //activates the components
                yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents, 
                    (float activationCost)=> //callback with the power used by components successfully activated
                        {
                            CurrentPower -= activationCost;
                            combatInterface.ShowPower(CurrentPower); 
                        }));
                //componentSelectionOn = false;
                firing = false;
                UnSelectComponents(false);
            }
            receivedMoveCommand = false;

            if(CurrentPower <=0) //end turn
            {
                continueTurn = false;
            }

            yield return null;
        }
        takingTurn = false;
        //de-activate GUI
        combatInterface.ShowComponentActivationButtons(null, null);
        combatInterface.ShowStatsPanel(false);

    }//ExecuteTurn

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

    #endregion PublicMethods


    #region PrivateMethods
    /// <summary>
    /// Start the component selection sequence - let's the player click on components in the component selection panel
    /// </summary>
    /// <returns></returns>
    private IEnumerator ComponentSelectionSequence()
    {
        Debug.Log("Selecting Components - [Space] to confirm");

        while (componentSelectionOn)
        {
            yield return null;
        }
        componentSelectionOn = false;
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
                totalActivationCost -= component.ActivationCost;
                combatInterface.ShowPower(CurrentPower - totalActivationCost);
            }
        }
    }//SelectComponent

    #region InternalCallbacks
    /// <summary>
    /// clicked on ground - move command
    /// </summary>
    /// <param name="worldPosition"></param>
    void SpaceGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("Click on ground at position: "+worldPosition);
        if (takingTurn && !receivedMoveCommand)
        {
            Debug.Log("Move command received " + shipBPMetaData.BlueprintName);
            shipMove.destination = worldPosition;
            Debug.Log("Distance to dest " + Vector3.Distance(trans.position, worldPosition));
            receivedMoveCommand = true;
        }
    }
    /// <summary>
    /// clicked on a component - toggle selection, start component selection sequnce if required
    /// </summary>
    /// <param name="component"></param>
    void OnComponentClicked(ShipComponent component)
    {
        if(firing)
        {
            return;
        }
        Debug.Log("At PlayerShip: "+ component.componentName + " clicked");
        componentSelectionOn = true;

        SelectComponent(component, !selectedComponents.Contains(component));
    }

    #endregion InternalCallbacks

    //Helper
    /// <summary>
    /// gets the lineRenderer prefab from Resources, instantiates and assigns to the playership
    /// </summary>
    private void ConfigurePlayerShip()
    {
        //add line renderer for movement and targeting
        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/LineRenderer");

        GameObject lineObj = Instantiate(linePrefab) as GameObject;
        lineObj.transform.SetParent(transform, false);
        line = lineObj.GetComponent<LineRenderer>();

        playerAttack.line = line;
        line.enabled = false;
    }
    #endregion PrivateMethods
    #endregion Methods
}
