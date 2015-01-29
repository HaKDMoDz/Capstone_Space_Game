using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    public PlayerAttack playerAttack { get; private set; }

    #region Internal
    private bool continueTurn = true;
    private bool receivedMoveCommand;
    private bool componentSelectionOn;
    private bool dragging;
    //private ShipComponent componentClickedOn;
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
            component.OnComponentClicked += OnComponentClicked;
            component.OnComponentMouseOver += OnComponentMouseOver;
        }
    }
    
    
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");
        continueTurn = true;

        CombatSystemInterface.Instance.EnableComponentSelectionPanel(true);

        while (continueTurn)
        {
            if (receivedMoveCommand)
            {
                yield return StartCoroutine(shipMove.Move());
                receivedMoveCommand = false;
#if FULL_DEBUG
                Debug.Log(shipBPMetaData.blueprintName + "- Movement end");
#endif
            }

            if (componentSelectionOn)
            {
                yield return StartCoroutine(ComponentSelectionSequence());
                yield return StartCoroutine(playerAttack.ActivateComponents(selectedComponents));
                Debug.Log("activating components");
            }

            yield return null;
        }
        UnSelectComponents();
    }

    private void UnSelectComponents()
    {
        foreach (ShipComponent component in shipBP.slot_component_table.Values)
        {
            component.Selected = false;
        }
        selectedComponents.Clear();
    }

    public void Move(Vector3 destination)
    {
        if (!receivedMoveCommand)
        {
            Debug.Log("Move command received " + shipBPMetaData.blueprintName);
            shipMove.destination = destination;
            receivedMoveCommand = true;
        }
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

    //ShipComponent CheckClickOnComponent()
    //{
    //    Debug.Log("checking click");
    //    ShipComponent componentClickedOn = null;

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, 1000f, 1 <<TagsAndLayers.ComponentsLayer))
    //    {
    //        Debug.Log(hit.collider.name);
    //        componentClickedOn = hit.collider.gameObject.GetComponent<ShipComponent>();
    //    }
    //    return componentClickedOn;
    //}


    #region InternalCallbacks

    void OnComponentClicked(ShipComponent component)
    {
        Debug.Log(component.componentName + " clicked");
        componentSelectionOn = true;
        //componentClickedOn = component;
        if (!selectedComponents.Contains(component))
        {
            selectedComponents.Add(component);
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
