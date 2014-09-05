using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerShip : TurnBasedUnit
{


    //cached components
    ShipMove shipMove;
    ShipAttack shipAttack;
    ShipBlueprint shipBlueprint;

    [SerializeField]
    Camera componentCamera;

    //book-keeping vars
    List<ShipComponent> selectedComponents;

    public override void Awake()
    {
        base.Awake();
        shipMove = gameObject.GetSafeComponent<ShipMove>();
        shipAttack = gameObject.GetSafeComponent<ShipAttack>();
        shipBlueprint = gameObject.GetSafeComponent<ShipBlueprint>();

        componentCamera.enabled = false;
        //manual init to ensure correct intitialization order
        shipBlueprint.Init();
        shipMove.Init();
        shipAttack.Init();

        selectedComponents = new List<ShipComponent>();
    }


    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (PlayerShip) starts turn");
        componentCamera.enabled = true;
        projector.enabled = true;

        yield return base.ExecuteTurn();

        //ends the player's turn only upon hitting Space
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            UnselectComponents();
            if (Input.GetMouseButtonUp(1))
            {
                yield return StartCoroutine(shipMove.Move(GetWorldCoordsFromMouse(Input.mousePosition)));
                
            }

            if (Input.GetMouseButtonDown(0))
            {
                ShipComponent firstComponent = CheckClickOnComponent();
                if (firstComponent)
                {
                    UnselectComponents();
                    yield return StartCoroutine(ComponentSelectionSequence(firstComponent));
                    yield return StartCoroutine(shipAttack.ActivateComponents(selectedComponents));
                }

            }
            if (Input.GetMouseButton(2))
            {
                CameraDirector.Instance.OrbitAroundImmediate(trans, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            yield return null;
        }

        projector.enabled = false;
        componentCamera.enabled = false;
        Debug.Log(unitName + " (PlayerShip) ends turn");
    }

    IEnumerator ComponentSelectionSequence(ShipComponent firstComponent)
    {
        Debug.Log("Selecting Components - [Enter] to confirm");
        bool dragging = true;
        ShipComponent componentClickedOn = firstComponent;
        Type selectedCompType = componentClickedOn.GetType();

        SelectComponent(componentClickedOn);
        yield return null;

        while (!Input.GetKeyDown(KeyCode.Return))
        {

            componentClickedOn = CheckClickOnComponent();


            if (Input.GetMouseButtonDown(0) && componentClickedOn)
            {
                dragging = true;
                if (componentClickedOn.GetType() == selectedCompType)
                {
                    SelectComponent(componentClickedOn);
                }
                else //start new selection
                {
                    UnselectComponents();
                    SelectComponent(componentClickedOn);
                    selectedCompType = componentClickedOn.GetType();
                }

            }

            if (dragging && Input.GetMouseButton(0) && componentClickedOn && componentClickedOn.GetType() == selectedCompType)
            {
                SelectComponent(componentClickedOn);
            }

            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }
            if (Input.GetMouseButton(2))
            {
                CameraDirector.Instance.OrbitAroundImmediate(trans, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            yield return null;
        }

    }
    void SelectComponent(ShipComponent comp)
    {
        if (!selectedComponents.Contains(comp))
        {
            selectedComponents.Add(comp);
            comp.Selected = true;

        }
    }
    void UnselectComponents()
    {
        foreach (ShipComponent comp in selectedComponents)
        {
            comp.Selected = false;
        }
        selectedComponents.Clear();
    }

    ShipComponent CheckClickOnComponent()
    {
        ShipComponent componentClickedOn = null;

        Ray ray = componentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << GlobalTagsAndLayers.Instance.layers.componentsLayer))
        {
            //Debug.Log(hit.collider.name);
            componentClickedOn = hit.collider.gameObject.GetComponent<ShipComponent>();
        }
        return componentClickedOn;
    }

    Vector3 GetWorldCoordsFromMouse(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;

        Vector3 worldCoords = trans.position;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << GlobalTagsAndLayers.Instance.layers.groundLayer))
        {
            worldCoords = hit.point;
            worldCoords.y = trans.position.y;
        }
        return worldCoords;
    }

}

