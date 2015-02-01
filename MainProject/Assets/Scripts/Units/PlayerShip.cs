using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    }

    private void RegisterInput(bool register)
    {
        registerInput = register;
        if (register)
        {
            foreach (ShipComponent component in components)
            {
                component.OnComponentClicked += OnComponentClicked;
                component.OnComponentMouseOver += OnComponentMouseOver;
            }
            spaceGround.OnGroundClick += SpaceGroundClick;

        }
        else
        {
            foreach (ShipComponent component in components)
            {
                component.OnComponentClicked -= OnComponentClicked;
                component.OnComponentMouseOver -= OnComponentMouseOver;
            }
            spaceGround.OnGroundClick -= SpaceGroundClick;
        }
    }


    public override IEnumerator ExecuteTurn()
    {
        RegisterInput(true);
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");
        continueTurn = true;

        CombatSystemInterface.Instance.EnableComponentSelectionPanel(true);

        while (continueTurn)
        {
            if (receivedMoveCommand)
            {
                RegisterInput(false);
                UnSelectComponents();
                yield return StartCoroutine(shipMove.Move());
#if FULL_DEBUG
                Debug.Log(shipBPMetaData.blueprintName + "- Movement end");
#endif
                RegisterInput(true);
            }

            if (componentSelectionOn)
            {
                yield return StartCoroutine(ComponentSelectionSequence());
                Debug.Log("activating components");
                RegisterInput(false);
                yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents));
                UnSelectComponents();
                RegisterInput(true);
            }
            receivedMoveCommand = false;
            yield return null;
        }
        RegisterInput(false);
    }

    private void UnSelectComponents()
    {
        foreach (ShipComponent component in components)
        {
            component.Selected = false;
        }
        selectedComponents.Clear();
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


    #region InternalCallbacks
    void SpaceGroundClick(Vector3 worldPosition)
    {
        //Debug.Log("Click on ground at position: "+worldPosition);
        if (!receivedMoveCommand)
        {
            Debug.Log("Move command received " + shipBPMetaData.blueprintName);
            shipMove.destination = worldPosition;
            receivedMoveCommand = true;
        }
    }

    void OnComponentClicked(ShipComponent component)
    {
        Debug.Log(component.componentName + " clicked");
        componentSelectionOn = true;
        //componentClickedOn = component;
        if (!selectedComponents.Contains(component))
        {
            selectedComponents.Add(component);
            component.Selected = true;
        }
        else
        {
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
