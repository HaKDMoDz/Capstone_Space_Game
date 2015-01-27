using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerShip : TurnBasedUnit
{
    #region Fields

    //references
    public ShipControlInterface shipControlInterface { get; private set; }

    #region Internal
    private bool receivedMoveCommand;
    #endregion Internal
    #endregion Fields

    #region Methods
    #region PublicMethods
    public void Init(ShipBlueprint shipBP, ShipMove shipMove, ShipControlInterface shipControlInterface)
    {
        base.Init(shipBP, shipMove);
        this.shipControlInterface = shipControlInterface;
    }
    public override IEnumerator ExecuteTurn()
    {
        yield return StartCoroutine(base.ExecuteTurn());
        Debug.Log("Player unit turn");

        CombatSystemInterface.Instance.EnableComponentSelectionPanel(true);

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if(receivedMoveCommand)
            {
                yield return StartCoroutine(shipMove.Move());
                receivedMoveCommand = false;
                #if FULL_DEBUG
                Debug.Log(shipBPMetaData.blueprintName+ "- Movement end");
                #endif
            }

            //if(Input.GetMouseButtonDown(0))
            //{
            //    //Debug.Log("Left click");
            //    ShipComponent comp = CheckClickOnComponent();
            //}

            yield return null;  
        }
    }
    ShipComponent CheckClickOnComponent()
    {
        ShipComponent componentClickedOn = null;

        Ray ray = componentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << TagsAndLayers.ComponentsLayer))
        {
#if FULL_DEBUG
            Debug.Log("Clicked on "+ hit.collider.name);
#endif
            componentClickedOn = hit.collider.gameObject.GetComponent<ShipComponent>();
        }
        return componentClickedOn;
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
    #endregion Methods
}
