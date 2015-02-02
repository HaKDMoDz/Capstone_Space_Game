using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    public PlayerAttack playerAttack { get; private set; }
    private SpaceGround spaceGround;

    #region Internal
    private bool continueTurn = true;
    private bool registerInput;
    private bool receivedMoveCommand;
    private bool componentSelectionOn;
    private bool dragging;
    private bool takingTurn = false;
    private bool firing = false;
    //private ShipComponent componentClickedOn;
    private List<ShipComponent> components = new List<ShipComponent>();
    private List<ShipComponent> selectedComponents = new List<ShipComponent>();

    #endregion Internal
    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, PlayerAttack playerAttack)
    {
        base.Init(shipBP, shipMove);
        this.playerAttack = playerAttack;
        this.playerAttack.Init();

        InputManager.Instance.RegisterKeysDown(
            (key) => { componentSelectionOn = false; },
            KeyCode.Space);

        InputManager.Instance.RegisterKeysDown(
            (key) => { continueTurn = false; },
           KeyCode.KeypadEnter, KeyCode.Return
            );

        foreach (ShipComponent component in shipBP.slot_component_table.Values)
        {
            component.Init();
            components.Add(component);

        }
        spaceGround = SpaceGround.Instance;
        foreach (ShipComponent component in components)
        {
            component.OnComponentClicked += OnComponentClicked;
            component.OnComponentMouseOver += OnComponentMouseOver;
        }
        spaceGround.OnGroundClick += SpaceGroundClick;
    }


    public override IEnumerator ExecuteTurn()
    {
        takingTurn = true;
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");
        continueTurn = true;

        CombatSystemInterface.Instance.EnableComponentSelectionPanel(true);
        CombatSystemInterface.Instance.ShowComponentActivationButtons( ActivateAllComponents, components);

        while (continueTurn)
        {
            if (receivedMoveCommand)
            {
                UnSelectComponents();
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
                yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents));
                firing = false;
                UnSelectComponents();
            }
            receivedMoveCommand = false;
            yield return null;
        }
        takingTurn = false;
        CombatSystemInterface.Instance.ShowComponentActivationButtons(null,null);
    }

    public void ActivateAllComponents(Type compType)
    {
        Debug.Log("Activating all " + compType.ToString());
    }

    #endregion PublicMethods


    #region PrivateMethods
    private IEnumerator ComponentSelectionSequence()
    {
        Debug.Log("Selecting Components - [Enter] to confirm");

        while (componentSelectionOn)
        {
            yield return null;
        }
        componentSelectionOn = false;
    }
    private void UnSelectComponents()
    {
        foreach (ShipComponent component in components)
        {
            component.Selected = false;
        }
        selectedComponents.Clear();
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
        //componentClickedOn = component;
        if (!selectedComponents.Contains(component))
        {
            Debug.Log("selected ");
            selectedComponents.Add(component);
            component.Selected = true;
        }
        else
        {
            Debug.Log("removed");
            selectedComponents.Remove(component);
            component.Selected = false;
        }
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
    #endregion PrivateMethods
    #endregion Methods
}
