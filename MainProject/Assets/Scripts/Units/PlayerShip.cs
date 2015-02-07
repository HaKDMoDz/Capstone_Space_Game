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

    #region Internal
    private bool continueTurn = true;
    private bool registerInput;
    private bool receivedMoveCommand;
    private bool componentSelectionOn;
    private bool dragging;
    private bool takingTurn = false;
    private bool firing = false;
    private float totalActivationCost = 0.0f;
    
    
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();


    #endregion Internal
    #endregion Fields

    #region Methods
    #region PublicMethods
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
            //component.OnComponentMouseOver += OnComponentMouseOver;
        }
        spaceGround.OnGroundClick += SpaceGroundClick;
    }


    public override IEnumerator ExecuteTurn()
    {
        takingTurn = true;
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");

        continueTurn = true;
        totalActivationCost = 0.0f;

        combatInterface.EnableComponentSelectionPanel(true);
        combatInterface.ShowStatsPanel(true);
        combatInterface.ShowComponentActivationButtons(SelectAllComponents, components);
        combatInterface.UpdatePower(CurrentPower);

        while (continueTurn)
        {
            if (receivedMoveCommand)
            {
                UnSelectComponents(false);
                yield return StartCoroutine(shipMove.Move());
                #if FULL_DEBUG
                Debug.Log(shipBPMetaData.blueprintName + "- Movement end");
                #endif
            }

            if (componentSelectionOn)
            {
                yield return StartCoroutine(ComponentSelectionSequence());
                Debug.Log("activating components");
                firing = true;
                yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents, 
                    (float activationCost)=>
                        {
                            CurrentPower -= activationCost;
                            combatInterface.UpdatePower(CurrentPower); 
                        }));
                //componentSelectionOn = false;
                firing = false;
                UnSelectComponents(false);
            }
            receivedMoveCommand = false;

            if(CurrentPower <=0)
            {
                continueTurn = false;
            }

            yield return null;
        }
        takingTurn = false;
        combatInterface.ShowComponentActivationButtons(null, null);
        combatInterface.ShowStatsPanel(false);
    }

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
                combatInterface.UpdatePower(CurrentPower - totalActivationCost);
            }
        }
        else
        {
            if(selectedComponents.Contains(component))
            {
                //Debug.Log("de-select");
                selectedComponents.Remove(component);
                component.Selected = false;
                totalActivationCost -= component.ActivationCost;
                combatInterface.UpdatePower(CurrentPower - totalActivationCost);
            }
        }
        
        
    }

    #region InternalCallbacks
    void SpaceGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("Click on ground at position: "+worldPosition);
        if (takingTurn && !receivedMoveCommand)
        {
            Debug.Log("Move command received " + shipBPMetaData.blueprintName);
            shipMove.destination = worldPosition;
            receivedMoveCommand = true;
        }
    }

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

    void OnComponentMouseOver(ShipComponent component)
    {
        //if (componentSelectionOn)
        //{
        //    Debug.Log(component.componentName + " entered");
        //    selectedComponents.Add(component);
        //}
    }

    #endregion InternalCallbacks

    //Helper
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
