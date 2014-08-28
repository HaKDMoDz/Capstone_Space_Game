using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        //manual init to ensure correct intitialization order
        shipBlueprint.Init();
        shipMove.Init();
        shipAttack.Init();

        selectedComponents = new List<ShipComponent>();
    }


    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (PlayerShip) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        //ends the player's turn only upon hitting Space
        while (!Input.GetKeyDown(KeyCode.Space))
        {

            if (Input.GetMouseButtonDown(1))
            {
                yield return StartCoroutine(shipMove.Move(GetWorldCoordsFromMouse(Input.mousePosition)));
            }
            //else if(Input.GetKeyDown(KeyCode.Alpha1))
            if (Input.GetMouseButtonDown(0) && CheckClickOnComponent())
            {
                selectedComponents.Clear();
                yield return StartCoroutine(ComponentSelectionSequence());


                //yield return StartCoroutine(shipAttack.Fire(GetWorldCoordsFromMouse(Input.mousePosition)));
            }
            //else if(Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    yield return StartCoroutine(shipAttack.FireMissiles(GetWorldCoordsFromMouse(Input.mousePosition)));
            //}
            
            
            yield return null;
        }

        projector.enabled = false;
        Debug.Log(unitName + " (PlayerShip) ends turn");
    }

    IEnumerator ComponentSelectionSequence()
    {
        bool dragging = true;
        //List<Vector3> dragPoints = new List<Vector3>();
        //List<Component> selectedComponents = new List<Component>();

        while(!Input.GetKeyDown(KeyCode.Return))
        {
            //mouse down
                //start drag
                //record position
           //mouse up
                //stop drag

            ShipComponent componentClickedOn = CheckClickOnComponent();

            if (Input.GetMouseButtonDown(0) && componentClickedOn)
            {
                
                dragging = true;
                if(!selectedComponents.Contains(componentClickedOn))
                {
                    selectedComponents.Add(componentClickedOn);
                }
            }
            //else
            //{
            //    dragging = false;
            //}
            if (dragging && Input.GetMouseButton(0) && componentClickedOn)
            {
                if (!selectedComponents.Contains(componentClickedOn))
                {
                    selectedComponents.Add(componentClickedOn);
                }
            }
            //else
            //{
            //    dragging = false;
            //}
            if(Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }

            yield return null;
        }

    }

    ShipComponent CheckClickOnComponent()
    {
        ShipComponent componentClickedOn = null;
        
        Ray ray = componentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, 1000f,1<<GlobalTagsAndLayers.Instance.layers.componentsLayer))
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

