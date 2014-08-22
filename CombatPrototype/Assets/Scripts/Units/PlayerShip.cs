using UnityEngine;
using System.Collections;

public class PlayerShip : TurnBasedUnit 
{

    public int groundLayer;


    //cached components
    ShipMove shipMove;

    public override void Awake()
    {
        base.Awake();
        shipMove = GetComponent<ShipMove>();
    }


    public override IEnumerator ExecuteTurn()
    {
        Debug.Log(unitName + " (PlayerShip) starts turn");
        projector.enabled = true;

        yield return base.ExecuteTurn();

        //ends the player's turn only upon hitting Space
        while(!Input.GetKeyDown(KeyCode.Space))
        {
        
            if(Input.GetMouseButtonDown(1))
            {
                shipMove.Move();
            }
            
            yield return null;
        }

        projector.enabled = false;
        Debug.Log(unitName + " (PlayerShip) ends turn");
    }

    Vector3 GetWorldCoordsFromMouse(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;

        Vector3 worldCoords = _trans.position;

        if (Physics.Raycast(ray, out hit, 1000f, 1 << groundLayer))
        {
            worldCoords = hit.point;
            worldCoords.y = _trans.position.y;
        }
        return worldCoords;
    }

}
